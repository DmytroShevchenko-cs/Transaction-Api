using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionApi.Model.Entity;
using TransactionApi.Service.Services.Interfaces;

namespace TransactionApi.Web.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class TransactionController(ITransactionService transactionService, IFileConversionService fileConversionService) : ControllerBase
{
    private readonly ITransactionService _transactionService = transactionService;
    private readonly IFileConversionService _fileConversionService = fileConversionService;

    /// <summary>
    /// Imports transaction data from a CSV file.
    /// </summary>
    /// <param name="file">The CSV file containing the transactions.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>Returns a status code 200 (OK) if the import is successful.</returns>
    [HttpPost]
    public async Task<IActionResult> ImportData(IFormFile file, CancellationToken cancellationToken)
    {
        List<TransactionEntity> transactions;
        await using (var stream = file.OpenReadStream())
        {
            transactions = await _fileConversionService.ReadCsvFileAsync(stream, cancellationToken);
        }

        await _transactionService.SaveToDbAsync(transactions, cancellationToken);
        
        return Ok();
    }
    
    /// <summary>
    /// Retrieves transactions within the specified date range.
    /// </summary>
    /// <param name="dateFrom">The start date of the range. (yyyy-mm-dd hh:mm:ss)</param>
    /// <param name="dateTo">The end date of the range. (yyyy-mm-dd hh:mm:ss)</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>Returns a list of transactions within the specified date range.</returns>
    [HttpGet]
    public async Task<IActionResult> GetTransactionsByDates([FromQuery]DateTime dateFrom, [FromQuery]DateTime dateTo, CancellationToken cancellationToken)
    {
        var transactions = await _transactionService.GetTransactionsByDatesAsync(dateFrom, dateTo, cancellationToken);
        return Ok(transactions);
    }
    
    /// <summary>
    /// Retrieves transactions by clients within the specified date range.
    /// </summary>
    /// <param name="dateFrom">The start date of the range. (yyyy-mm-dd hh:mm:ss)</param>
    /// <param name="dateTo">The end date of the range. (yyyy-mm-dd hh:mm:ss)</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>Returns a list of client transactions within the specified date range.</returns>
    [HttpGet("clients")]
    public async Task<IActionResult> GetTransactionsByUserDates([FromQuery]DateTime dateFrom, [FromQuery]DateTime dateTo, CancellationToken cancellationToken)
    {
        var transactions = await _transactionService.GetTransactionsByUserDatesAsync(dateFrom, dateTo, cancellationToken);
        return Ok(transactions);
    }
    
    /// <summary>
    /// Exports transactions within the specified date range to an Excel file.
    /// </summary>
    /// <param name="dateFrom">The start date of the range. (yyyy-mm-dd hh:mm:ss)</param>
    /// <param name="dateTo">The end date of the range. (yyyy-mm-dd hh:mm:ss)</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>Returns an Excel file with transactions within the specified date range.</returns>
    [HttpGet("export")]
    public async Task<IActionResult> ExportTransactionsByDates([FromQuery]DateTime dateFrom, [FromQuery]DateTime dateTo, CancellationToken cancellationToken)
    {
        var transactions = await _transactionService.GetTransactionsByDatesAsync(dateFrom, dateTo, cancellationToken);
        var xlsx = await _fileConversionService.ConvertToExcelAsync(transactions, cancellationToken);
        return File(xlsx, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "transactions.xlsx");
    }
    
    /// <summary>
    /// Exports transactions within the specified date range to an Excel file.
    /// </summary>
    /// <param name="dateFrom">The start date of the range. (yyyy-mm-dd hh:mm:ss)</param>
    /// <param name="dateTo">The end date of the range. (yyyy-mm-dd hh:mm:ss)</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>Returns an Excel file with transactions within the specified date range.</returns>
    [HttpGet("export/clients")]
    public async Task<IActionResult> ExportTransactionsByUserDates([FromQuery]DateTime dateFrom, [FromQuery]DateTime dateTo, CancellationToken cancellationToken)
    {
        var transactions = await _transactionService.GetTransactionsByUserDatesAsync(dateFrom, dateTo, cancellationToken);
        var xlsx = await _fileConversionService.ConvertToExcelAsync(transactions, cancellationToken);
        return File(xlsx, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "transactions.xlsx");
    }
    
    /// <summary>
    /// Retrieves transactions by clients within january 2024.
    /// </summary>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>Returns a list of client transactions within january 2024.</returns>
    [HttpGet("january/clients")]
    public async Task<IActionResult> GetJanuaryTransactions(CancellationToken cancellationToken)
    {
        var transactions = await _transactionService.GetTransactionsByDatesAsync(DateTime.Parse("2024-01-01 00:00:00"), DateTime.Parse("2024-01-31 00:00:00"), cancellationToken);
        return Ok(transactions);
    }
    
    /// <summary>
    /// Exports transactions within january 2024 to an Excel file.
    /// </summary>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>Returns an Excel file with transactions within tjanuary 2024.</returns>
    [HttpGet("export/january/clients")]
    public async Task<IActionResult> ExportJanuaryTransactions(CancellationToken cancellationToken)
    {
        var transactions = await _transactionService.GetTransactionsByDatesAsync(DateTime.Parse("2024-01-01 00:00:00"), DateTime.Parse("2024-01-31 00:00:00"), cancellationToken);
        var xlsx = await _fileConversionService.ConvertToExcelAsync(transactions, cancellationToken);
        return File(xlsx, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "transactions.xlsx");
    }
    
}