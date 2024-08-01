namespace TransactionApi.DAL.Entity;

public class TransactionEntity
{
    public string TransactionId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string ClientLocation { get; set; } = null!;
}