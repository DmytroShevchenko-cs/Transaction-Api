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
                            USING (VALUES (@TransactionId, @Name, @Email, @Amount, @TransactionDate, @ClientLocation, @TimeZone)) AS source (TransactionId, Name, Email, Amount, TransactionDate, ClientLocation, TimeZone)
                            ON target.TransactionId = source.TransactionId
                            WHEN MATCHED THEN
                                UPDATE SET Name = source.Name,
                                           Email = source.Email,
                                           Amount = source.Amount,
                                           TransactionDate = source.TransactionDate,
                                           ClientLocation = source.ClientLocation,
                                           TimeZone = source.TimeZone
                            WHEN NOT MATCHED THEN
                                INSERT (TransactionId, Name, Email, Amount, TransactionDate, ClientLocation, TimeZone)
                                VALUES (source.TransactionId, source.Name, source.Email, source.Amount, source.TransactionDate, source.ClientLocation, source.TimeZone);";


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
    public async Task<List<TransactionEntity>> GetTransactionsByUserDatesAsync(DateTime from, DateTime to,
        CancellationToken cancellationToken = default)
    {
        var userCoordinates = await geolocationApiService.GetClientTimeZone(cancellationToken);
        var userTimeZone = DateTimeZoneProviders.Tzdb[userCoordinates];
        
        //for include date in query to db
        to = to.AddDays(1);
        
        var fromLocal = LocalDateTime.FromDateTime(from).InZoneLeniently(userTimeZone);
        var toLocal = LocalDateTime.FromDateTime(to.AddDays(1)).InZoneLeniently(userTimeZone);
    
        // Convert the LocalDateTime to UTC for querying the database
        var fromUtc = fromLocal.ToInstant().ToDateTimeUtc();
        var toUtc = toLocal.ToInstant().ToDateTimeUtc();
        
        await using (var connection = new SqlConnection(_connectionString))
        {
            var query = @"
            SELECT * FROM Transactions
            WHERE TransactionDate BETWEEN @From AND @To";
    
            var transactions = (await connection.QueryAsync<TransactionEntity>(query,
                new { From = fromUtc, To = toUtc })).ToList();
            
            var filteredTransactions = transactions.Where(transaction =>
            {
                var transactionTimeZone = DateTimeZoneProviders.Tzdb[transaction.TimeZone];
                var transactionZonedDateTime = LocalDateTime.FromDateTime(transaction.TransactionDate)
                    .InZoneLeniently(transactionTimeZone);
    
                // Convert the transaction time to the user's time zone
                var convertedDateTime = transactionZonedDateTime.WithZone(userTimeZone);
    
                // Check if the converted transaction time falls within the user's specified range
                return convertedDateTime.ToDateTimeUnspecified() >= from 
                       && convertedDateTime.ToDateTimeUnspecified() < to;
            }).ToList();
            
            foreach (var transaction in transactions.Except(filteredTransactions))
            {
                var transactionTimeZone = DateTimeZoneProviders.Tzdb[transaction.TimeZone];
                var dateTime = transaction.TransactionDate.ToLocalDateTime();
                
                var transactionZonedDateTime = dateTime.InZoneLeniently(transactionTimeZone);
                var convertedDateTime = transactionZonedDateTime.WithZone(userTimeZone);
                
                if (convertedDateTime.ToDateTimeUtc() >= from && convertedDateTime.ToDateTimeUtc() <= to)
                {
                    filteredTransactions.Add(transaction);
                }
            }
    
            return filteredTransactions;
        }
    }
}