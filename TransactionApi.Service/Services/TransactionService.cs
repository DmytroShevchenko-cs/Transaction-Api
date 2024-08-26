using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using NodaTime;
using NodaTime.Extensions;
using Npgsql;
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
        
        await using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            await using (var transaction = await connection.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    foreach (var record in transactions)
                    {
                        var query = @"
                            INSERT INTO ""Transactions"" (""TransactionId"", ""Name"", ""Email"", ""Amount"", ""TransactionDate"", ""ClientLocation"", ""TimeZone"")
                            VALUES (@TransactionId, @Name, @Email, @Amount, @TransactionDate, @ClientLocation, @TimeZone)
                            ON CONFLICT (""TransactionId"") 
                            DO UPDATE SET
                                ""Name"" = EXCLUDED.""Name"",
                                ""Email"" = EXCLUDED.""Email"",
                                ""Amount"" = EXCLUDED.""Amount"",
                                ""TransactionDate"" = EXCLUDED.""TransactionDate"",
                                ""ClientLocation"" = EXCLUDED.""ClientLocation"",
                                ""TimeZone"" = EXCLUDED.""TimeZone"";";
                        
                        await connection.ExecuteAsync(query, record, transaction, commandType: CommandType.Text);
                    }

                    await transaction.CommitAsync(cancellationToken);
                }
                catch
                {
                    await transaction.CommitAsync(cancellationToken);
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
        await using (var connection = new NpgsqlConnection(_connectionString))
        {
            var query = $@"
                SELECT * FROM ""Transactions""
                WHERE ""TransactionDate"" BETWEEN '{from}' AND '{to}'";

            var transactions = await connection.QueryAsync<TransactionEntity>(query);
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

        
        await using (var connection = new NpgsqlConnection(_connectionString))
        {
            var query = $@"SELECT 
                    ""TransactionId"", 
                    ""Name"", 
                    ""Email"", 
                    ""Amount"", 
                    ""TransactionDate""::timestamp AS ""TransactionDate"",
                    ""ClientLocation"",
                    ""TimeZone""
                FROM public.""Transactions""
                WHERE ""TransactionDate"" AT TIME ZONE ""TimeZone"" AT TIME ZONE 'UTC'
                BETWEEN '{fromUtc}' AND '{toUtc}';";
            
            var transactions = await connection.QueryAsync<TransactionEntity>(query);
            
            return transactions.ToList();
        }
    }
}