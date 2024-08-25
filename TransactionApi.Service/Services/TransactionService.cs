using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using NodaTime;
using NodaTime.Extensions;
using TransactionApi.Model.Entity;
using TransactionApi.Service.Options;
using TransactionApi.Service.Services.Interfaces;

namespace TransactionApi.Service.Services;

public class TransactionService(IOptions<DbConnection> connectionString, IGeolocationApiService geolocationApiService)
    : ITransactionService
{
    private readonly string _connectionString = connectionString.Value.ConnectionString;

    /// <summary>
    /// Returns list of Transactions from file
    /// </summary>
    public async Task<IEnumerable<TransactionEntity>> SaveToDbAsync(List<TransactionEntity> transactions,
        CancellationToken cancellationToken = default)
    {

        foreach (var transaction in transactions)
        {
            transaction.TimeZone =
                await geolocationApiService.GetTimeZoneByLocation(transaction.ClientLocation, cancellationToken);
            
            DateTimeZone timeZone = DateTimeZoneProviders.Tzdb[transaction.TimeZone];
            ZonedDateTime zonedDateTime = transaction.TransactionDate.ToLocalDateTime().InZoneLeniently(timeZone);
            transaction.DateTimeUtc = zonedDateTime.ToDateTimeUtc();
        }
        
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
                            USING (VALUES (@TransactionId, @Name, @Email, @Amount, @TransactionDate, @ClientLocation, @TimeZone, @DateTimeUtc)) 
                            AS source (TransactionId, Name, Email, Amount, TransactionDate, ClientLocation, TimeZone, DateTimeUtc)
                            ON target.TransactionId = source.TransactionId
                            WHEN MATCHED THEN
                                UPDATE SET Name = source.Name,
                                           Email = source.Email,
                                           Amount = source.Amount,
                                           TransactionDate = source.TransactionDate,
                                           ClientLocation = source.ClientLocation,
                                           TimeZone = source.TimeZone,
                                           DateTimeUtc = source.DateTimeUtc
                            WHEN NOT MATCHED THEN
                                INSERT (TransactionId, Name, Email, Amount, TransactionDate, ClientLocation, TimeZone, DateTimeUtc)
                                VALUES (source.TransactionId, source.Name, source.Email, source.Amount, source.TransactionDate, source.ClientLocation, source.TimeZone, source.DateTimeUtc);";


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
    /// Returns list of Transactions by dates with including user's time zone
    /// </summary>
    public async Task<List<TransactionEntity>> GetTransactionsByUserDatesAsync(DateTime from, DateTime to, string userTimeZoneId,
        CancellationToken cancellationToken = default)
    {
        var userTimeZone = DateTimeZoneProviders.Tzdb[userTimeZoneId];
        
        var fromUtc = LocalDateTime.FromDateTime(from).InZoneLeniently(userTimeZone).ToDateTimeUtc();
        var toUtc = LocalDateTime.FromDateTime(to).InZoneLeniently(userTimeZone).ToDateTimeUtc();
        
        await using (var connection = new SqlConnection(_connectionString))
        {
            var query = @"
                SELECT * FROM Transactions
                WHERE DateTimeUtc BETWEEN @From AND @To";
    
            var transactions = await connection.QueryAsync<TransactionEntity>(query,
                new { From = fromUtc, To = toUtc });
            
            return transactions.ToList();
        }
    }
}