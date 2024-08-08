using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionApi.Service.Services.Interfaces;

namespace TransactionApi.Web.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class TransactionController(ITransactionService transactionService) : ControllerBase
{
    private readonly ITransactionService _transactionService = transactionService;

    [HttpPost]
    public async Task<IActionResult> ImportData(IFormFile file, CancellationToken cancellationToken)
    {
        await using (var stream = file.OpenReadStream())
        {
            await _transactionService.SaveDbFromCsvAsync(stream, cancellationToken);
        }
        
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTransactionsByDates([FromQuery]DateTime dateFrom, [FromQuery]DateTime dateTo, CancellationToken cancellationToken)
    {
        var transactions = await _transactionService.GetTransactionsByDatesAsync(dateFrom, dateTo, cancellationToken);
        return Ok(transactions);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTransactionsByClientDates([FromQuery]DateTime dateFrom, [FromQuery]DateTime dateTo, CancellationToken cancellationToken)
    {
        var transactions = await _transactionService.GetTransactionsByClientDatesAsync(dateFrom, dateTo, cancellationToken);
        return Ok(transactions);
    }
    
    
    [HttpGet]
    public async Task<IActionResult> ExportTransactionsByDates([FromQuery]DateTime dateFrom, [FromQuery]DateTime dateTo, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet]
    public async Task<IActionResult> ExportTransactionsByClientDates([FromQuery]DateTime dateFrom, [FromQuery]DateTime dateTo, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
}