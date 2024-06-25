namespace BankingServiceAPI.Models;

public class TransactionLog
{
    public int TransactionId { get; set; }
    public BankTransaction? BankTransaction { get; set; }
    public int BankTransactionId { get; set; }
    public DateTime TimeDate { get; set; } = DateTime.Now;
}