namespace BankingServiceAPI.Models;

public class TransactionLog
{
    public int Id { get; init; }
    public int BankTransactionId { get; private set; }
    public BankTransaction? BankTransaction { get; private set; }
    public string? TransactionType { get; private set; }
    public decimal Amount { get; private set; }
    public int AccountOriginId { get; private set; }
    public int? AccountDestinationId { get; private set; }
    public DateTime TransactionDate { get; private set; } = DateTime.Now;
    public TransactionDetails? TransactionDetails { get; private set; }
    public TransactionAudit? TransactionAudit { get; private set; }

    private void SetBankTransactionId(int bankTransactionId) => BankTransactionId = bankTransactionId;
    public void SetBankTransaction(BankTransaction bankTransaction) => BankTransaction = bankTransaction;
    private void SetTransactionType(string? transactionType) => TransactionType = transactionType;
    private void SetAmount(decimal amount) => Amount = amount;
    private void SetAccountOriginId(int accountOriginId) => AccountOriginId = accountOriginId;
    private void SetAccountDestinationId(int? accountDestinationId) => AccountDestinationId = accountDestinationId;
    private void SetTransactionDate(DateTime transactionDate) => TransactionDate = transactionDate;
    private void SetTransactionDetails(TransactionDetails details) => TransactionDetails = details;
    private void SetTransactionAudit(TransactionAudit audit) => TransactionAudit = audit;

    public void Configure(int bankTransactionId, string transactionType, decimal amount, int accountOriginId,
        int? accountDestinationId, DateTime transactionDate, TransactionDetails details, TransactionAudit audit)
    {
        SetBankTransactionId(bankTransactionId);
        SetTransactionType(transactionType);
        SetAmount(amount);
        SetAccountOriginId(accountOriginId);
        SetAccountDestinationId(accountDestinationId);
        SetTransactionDate(transactionDate);
        SetTransactionDetails(details);
        SetTransactionAudit(audit);
    }
}