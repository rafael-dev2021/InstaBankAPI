namespace BankingServiceAPI.Models;

public class TransactionLog
{
    public int Id { get; init; }
    public int BankTransactionId { get; init; }
    public BankTransaction? BankTransaction { get; init; }
    public string? TransactionType { get; init; } 
    public string? Description { get; init; }
    public decimal Amount { get; init; } 
    public int AccountOriginId { get; init; } 
    public int? AccountDestinationId { get; init; } 
    public DateTime TransactionDate { get; init; } = DateTime.Now; 
}