using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TransactionApi.Model;
using TransactionApi.Model.Entity;
using TransactionApi.Service.Services;
using TransactionApi.Service.Services.Interfaces;
using TransactionApi.Test.Helpers;

namespace TransactionApi.Test.Services;

public class TransactionServiceTest : DefaultServiceTest<ITransactionService, TransactionService>
{
    private const string ConnectionString = "Server=.;Database=TestTransactionDB;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True";
    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<IGeolocationApiService, GeolocationApiService>();
        services.AddTransient<IFileConversionService, FileConversionService>();
        base.SetUpAdditionalDependencies(services);
    }

    [OneTimeSetUp]
    public async Task SetUp()
    {
        await using (var connection = new SqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"
                IF OBJECT_ID('Transactions', 'U') IS NOT NULL
                DROP TABLE Transactions;
                
                CREATE TABLE Transactions (
                    TransactionId NVARCHAR(50) PRIMARY KEY,
                    Name NVARCHAR(100),
                    Email NVARCHAR(100),
                    Amount DECIMAL(18, 2),
                    TransactionDate DATETIME,
                    ClientLocation NVARCHAR(100)
                )");
            
            var testData = TestDataHelper.CreateTestData();
            foreach (var transaction in testData)
            {
                var query = @"
                    INSERT INTO Transactions (TransactionId, Name, Email, Amount, TransactionDate, ClientLocation)
                    VALUES (@TransactionId, @Name, @Email, @Amount, @TransactionDate, @ClientLocation)";

                await connection.ExecuteAsync(query, transaction);
            }
        }
    }
    
    [OneTimeTearDown]
    public async Task TearDown()
    {
        await using (var connection = new SqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync("DROP TABLE IF EXISTS Transactions");
        }
    }

    [Test]
    [TestCase("2024-01-01 00:00:00", "2024-02-01 00:00:00")]// for january 2024
    [TestCase("2024-01-01 00:00:00", "2024-01-03 00:00:00")]
    [TestCase("2024-05-01 00:00:00", "2024-07-10 00:00:00")]
    public async Task GetTransactionByDates(DateTime dateTimeFrom, DateTime dateTimeTo)
    {
        var transactions = await Service.GetTransactionsByDatesAsync(dateTimeFrom, dateTimeTo);
        Assert.That(!transactions.Any(r => r.TransactionDate < dateTimeFrom &&  r.TransactionDate > dateTimeTo));
    } 
    [Test]
    [TestCase("2024-01-01 00:00:00", "2024-01-31 00:00:00")]// for january 2024
    [TestCase("2024-01-01 00:00:00", "2024-01-03 00:00:00")]
    [TestCase("2024-05-01 00:00:00", "2024-07-10 00:00:00")]
    public async Task GetTransactionByDatesWithLocation(DateTime dateTimeFrom, DateTime dateTimeTo)
    {
        var transactions = await Service.GetTransactionsByClientDatesAsync(dateTimeFrom, dateTimeTo);
        Assert.That(!transactions.Any(r => r.TransactionDate < dateTimeFrom.AddHours(-26) &&  r.TransactionDate > dateTimeTo.AddHours(26)));
    } 
}