

using TransactionApi.Model.Entity;

namespace TransactionApi.Service.Services.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<TransactionEntity>> SaveToDbAsync(List<TransactionEntity> transactions, CancellationToken cancellationToken = default);
    Task ExportExelAsync(CancellationToken cancellationToken = default);

    Task<List<TransactionEntity>> GetTransactionsByDatesAsync(DateTime from, DateTime to, 
        CancellationToken cancellationToken = default);
    
    Task<List<TransactionEntity>> GetTransactionsByClientDatesAsync(DateTime from, DateTime to,
        CancellationToken cancellationToken = default);
}