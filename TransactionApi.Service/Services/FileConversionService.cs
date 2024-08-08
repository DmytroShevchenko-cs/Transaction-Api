using System.Globalization;
using ClosedXML.Excel;
using CsvHelper;
using TransactionApi.Model.Entity;
using TransactionApi.Service.Services.Interfaces;

namespace TransactionApi.Service.Services;

public class FileConversionService : IFileConversionService
{
    public async Task<List<TransactionEntity>> ReadCsvFileAsync(Stream file, CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(file);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<TransactionEntityMap>();

        var records = new List<TransactionEntity>();

        await foreach (var record in csv.GetRecordsAsync<TransactionEntity>(cancellationToken))
        {
            records.Add(record);
        }

        return records;

    }

    public async Task<byte[]> ConvertToExcelAsync(List<TransactionEntity> transactions, CancellationToken cancellationToken = default)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Transactions");

            // headers
            worksheet.Cell(1, 1).Value = "Transaction ID";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Email";
            worksheet.Cell(1, 4).Value = "Amount";
            worksheet.Cell(1, 5).Value = "Transaction Date";

            // data
            var row = 2;
            foreach (var transaction in transactions)
            {
                worksheet.Cell(row, 1).Value = transaction.TransactionId;
                worksheet.Cell(row, 2).Value = transaction.Name;
                worksheet.Cell(row, 3).Value = transaction.Email;
                worksheet.Cell(row, 4).Value = transaction.Amount;
                worksheet.Cell(row, 5).Value = transaction.TransactionDate;
                row++;
            }

            await using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return await Task.FromResult(stream.ToArray());
            }
        }
    }
}