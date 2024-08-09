using CsvHelper.Configuration;
using TransactionApi.Model.Helpers;

namespace TransactionApi.Model.Entity;

public sealed class TransactionEntityMap : ClassMap<TransactionEntity>
{
    public TransactionEntityMap()
    {
        Map(m => m.TransactionId).Name("transaction_id");
        Map(m => m.Name).Name("name");
        Map(m => m.Email).Name("email");
        Map(m => m.Amount).Name("amount").TypeConverter<CustomDecimalConverter>();;
        Map(m => m.TransactionDate).Name("transaction_date");
        Map(m => m.ClientLocation).Name("client_location");
    }
}