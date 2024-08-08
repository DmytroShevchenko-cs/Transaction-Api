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
    
    [HttpGet]
    public async Task<IActionResult> GetTransactionsByDates([FromQuery]DateTime dateFrom, [FromQuery]DateTime dateTo, CancellationToken cancellationToken)
    {
        var transactions = await _transactionService.GetTransactionsByDatesAsync(dateFrom, dateTo, cancellationToken);
        return Ok(transactions);
    }
    
    [HttpGet("clients")]
    public async Task<IActionResult> GetTransactionsByClientDates([FromQuery]DateTime dateFrom, [FromQuery]DateTime dateTo, CancellationToken cancellationToken)
    {
        var transactions = await _transactionService.GetTransactionsByClientDatesAsync(dateFrom, dateTo, cancellationToken);
        return Ok(transactions);
    }
    
    
    [HttpGet("export")]
    public async Task<IActionResult> ExportTransactionsByDates([FromQuery]DateTime dateFrom, [FromQuery]DateTime dateTo, CancellationToken cancellationToken)
    {
        var transactions = await _transactionService.GetTransactionsByDatesAsync(dateFrom, dateTo, cancellationToken);
        var xlsx = await _fileConversionService.ConvertToExcelAsync(transactions, cancellationToken);
        return File(xlsx, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "transactions.xlsx");
    }
    
    [HttpGet("export/clients")]
    public async Task<IActionResult> ExportTransactionsByClientDates([FromQuery]DateTime dateFrom, [FromQuery]DateTime dateTo, CancellationToken cancellationToken)
    {
        var transactions = await _transactionService.GetTransactionsByClientDatesAsync(dateFrom, dateTo, cancellationToken);
        var xlsx = await _fileConversionService.ConvertToExcelAsync(transactions, cancellationToken);
        return File(xlsx, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "transactions.xlsx");
    }
    
}