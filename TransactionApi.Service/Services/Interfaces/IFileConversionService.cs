using TransactionApi.Model.Entity;

namespace TransactionApi.Service.Services.Interfaces;

public interface IFileConversionService
{
    public Task<List<TransactionEntity>> ReadCsvFileAsync(Stream file, CancellationToken cancellationToken = default);
    public Task<byte[]> ConvertToExcelAsync (List<TransactionEntity> transactions, CancellationToken cancellationToken = default);
}