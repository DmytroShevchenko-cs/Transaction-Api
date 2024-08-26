using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Extensions;
using Npgsql;
using NUnit.Framework;
using TransactionApi.Model;
using TransactionApi.Model.Entity;
using TransactionApi.Service.Services;
using TransactionApi.Service.Services.Interfaces;
using TransactionApi.Test.Helpers;

namespace TransactionApi.Test.Services;

public class TransactionServiceTest : DefaultServiceTest<ITransactionService, TransactionService>
{
    private const string ConnectionString = "Host=localhost;Database=TestTransactionDb;Username=postgres;Password=verysecurepass";
    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<IGeolocationApiService, GeolocationApiService>();
        services.AddTransient<IFileConversionService, FileConversionService>();
        base.SetUpAdditionalDependencies(services);
    }

    [OneTimeSetUp]
    public async Task SetUp()
    {
        await using (var connection = new NpgsqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"
                DROP TABLE IF EXISTS ""Transactions"";

                CREATE TABLE ""Transactions"" (
                    ""TransactionId"" VARCHAR(50) PRIMARY KEY,
                    ""Name"" VARCHAR(100),
                    ""Email"" VARCHAR(100),
                    ""Amount"" DECIMAL(18, 2),
                    ""TransactionDate"" TIMESTAMP,
                    ""ClientLocation"" VARCHAR(100),
                    ""TimeZone"" VARCHAR(100)
                )");
            var testData = TestDataHelper.CreateTestData();
            
            foreach (var transaction in testData)
            {
                var query = @"
                    INSERT INTO ""Transactions"" (""TransactionId"", ""Name"", ""Email"", ""Amount"", ""TransactionDate"", ""ClientLocation"", ""TimeZone"")
                    VALUES (@TransactionId, @Name, @Email, @Amount, @TransactionDate, @ClientLocation, @TimeZone)";

                await connection.ExecuteAsync(query, transaction);
            }
        }
    }
    
    [OneTimeTearDown]
    public async Task TearDown()
    {
        await using (var connection = new NpgsqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync("DROP TABLE IF EXISTS Transactions");
        }
    }

    [Test]
    [TestCase("2024-01-01 00:00:00", "2024-02-01 23:59:59")]// for january 2024
    [TestCase("2024-01-01 00:00:00", "2024-01-03 23:59:59")]
    [TestCase("2024-05-01 00:00:00", "2024-07-10 23:59:59")]
    public async Task GetTransactionByDates(DateTime dateTimeFrom, DateTime dateTimeTo)
    {
        var transactions = await Service.GetTransactionsByDatesAsync(dateTimeFrom, dateTimeTo);
        Assert.That(!transactions.Any(r => r.TransactionDate < dateTimeFrom &&  r.TransactionDate > dateTimeTo));
    } 
    
    [Test]
    [TestCase("2024-01-01 00:00:00", "2024-01-31 23:59:59", "Europe/Kiev")] // for January 2024
    [TestCase("2024-01-01 00:00:00", "2024-01-03 23:59:59", "America/New_York")]
    [TestCase("2024-05-01 00:00:00", "2024-07-10 23:59:59", "Asia/Tokyo")]
    public async Task GetTransactionByDatesWithLocation(DateTime dateTimeFrom, DateTime dateTimeTo, string userTimeZoneId)
    {
        var userTimeZone = DateTimeZoneProviders.Tzdb[userTimeZoneId];
        var fromUtc = LocalDateTime.FromDateTime(dateTimeFrom).InZoneLeniently(userTimeZone).ToDateTimeUtc();
        var toUtc = LocalDateTime.FromDateTime(dateTimeTo).InZoneLeniently(userTimeZone).ToDateTimeUtc();
        
        var transactions = await Service.GetTransactionsByUserDatesAsync(dateTimeFrom, dateTimeTo, userTimeZoneId);
        Assert.That(transactions.All(transaction => IsWithinUtcRange(transaction.TransactionDate, fromUtc, toUtc, userTimeZone)));
    } 
    
    private DateTime ConvertToUtc(DateTime dateTime, DateTimeZone timeZone)
    {
        return LocalDateTime.FromDateTime(dateTime)
            .InZoneLeniently(timeZone)
            .ToDateTimeUtc();
    }

    private bool IsWithinUtcRange(DateTime transactionDate, DateTime fromUtc, DateTime toUtc, DateTimeZone userTimeZone)
    {
        var transactionUtc = LocalDateTime.FromDateTime(transactionDate)
            .InZoneLeniently(userTimeZone)
            .ToDateTimeUtc();
    
        return transactionUtc >= fromUtc && transactionUtc <= toUtc;
    }
}