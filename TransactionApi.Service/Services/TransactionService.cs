using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using TransactionApi.Model.Entity;
using TransactionApi.Service.Options;
using TransactionApi.Service.Services.Interfaces;

namespace TransactionApi.Service.Services;

public class TransactionService(IOptions<DbConnection> connectionString, IGeolocationApiService geolocationApiService, IFileConversionService fileConversionService)
    : ITransactionService
{
    private readonly string _connectionString = connectionString.Value.ConnectionString;

    /// <summary>
    /// Returns list of Transactions from file
    /// </summary>
    public async Task<IEnumerable<TransactionEntity>> SaveToDbAsync(List<TransactionEntity> transactions,
        CancellationToken cancellationToken = default)
    {
        await using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            await using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    foreach (var record in transactions)
                    {
                        var query = @"
                            MERGE INTO Transactions AS target
                            USING (VALUES (@TransactionId, @Name, @Email, @Amount, @TransactionDate, @ClientLocation)) AS source (TransactionId, Name, Email, Amount, TransactionDate, ClientLocation)
                            ON target.TransactionId = source.TransactionId
                            WHEN MATCHED THEN
                                UPDATE SET Name = source.Name,
                                           Email = source.Email,
                                           Amount = source.Amount,
                                           TransactionDate = source.TransactionDate,
                                           ClientLocation = source.ClientLocation
                            WHEN NOT MATCHED THEN
                                INSERT (TransactionId, Name, Email, Amount, TransactionDate, ClientLocation)
                                VALUES (source.TransactionId, source.Name, source.Email, source.Amount, source.TransactionDate, source.ClientLocation);";


                        await connection.ExecuteAsync(query, record, transaction: transaction,
                            commandType: CommandType.Text);
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        return transactions;
    }

    /// <summary>
    /// Returns file with Transactions
    /// </summary>
    public Task ExportExelAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns list of Transactions by dates
    /// </summary>
    public async Task<List<TransactionEntity>> GetTransactionsByDatesAsync(DateTime from, DateTime to,
        CancellationToken cancellationToken = default)
    {
        await using (var connection = new SqlConnection(_connectionString))
        {
            var query = @"
                SELECT * FROM Transactions
                WHERE TransactionDate BETWEEN @From AND @To";

            var transactions = await connection.QueryAsync<TransactionEntity>(query, new { From = from, To = to });
            return transactions.ToList();
        }
    }

    /// <summary>
    /// Returns list of Transactions by dates and client time zone
    /// </summary>
    public async Task<List<TransactionEntity>> GetTransactionsByClientDatesAsync(DateTime from, DateTime to,
        CancellationToken cancellationToken = default)
    {
        var userCoordinates = await geolocationApiService.GetClientTimeZone(cancellationToken);
        
        //for include date in query to db
        to = to.AddDays(1);
        
        await using (var connection = new SqlConnection(_connectionString))
        {
            var query = @"
                SELECT * FROM Transactions
                WHERE TransactionDate BETWEEN @From AND @To";

            // 26 is max time difference in the world
            var transactions =
                (await connection.QueryAsync<TransactionEntity>(query,
                    new { From = from.AddHours(-26), To = to.AddHours(26) })).ToList();

            var filteredTransactions = transactions.Where(r =>
                r.TransactionDate >= from.AddHours(26) && r.TransactionDate <= to.AddHours(-26)).ToList();

            foreach (var transaction in transactions.Except(filteredTransactions))
            {
                var clientTransactionDate = await geolocationApiService.ConvertTimeByCoordinatesAsync(
                    transaction.ClientLocation,
                    userCoordinates, transaction.TransactionDate, cancellationToken);

                if (clientTransactionDate >= from && clientTransactionDate <= to)
                {
                    filteredTransactions.Add(transaction);
                }
            }

            return filteredTransactions;
        }
    }
}