namespace BankingServiceAPI.Models;

public class BankTransaction
{
    public int Id { get; protected set; }
    public BankAccount? AccountOrigin { get; protected set; }
    public int AccountOriginId { get; protected set; }
    public BankAccount? AccountDestination { get; protected set; }
    public int AccountDestinationId { get; protected set; }
    public decimal Amount { get; protected set; }
    public DateTime TransferDate { get; protected set; }

    public void SetId(int id) => Id = id;
    public void SetAccountOrigin(BankAccount accountOrigin) => AccountOrigin = accountOrigin;
    public void SetAccountOriginId(int accountOriginId) => AccountOriginId = accountOriginId;
    public void SetAccountDestination(BankAccount accountDestination) => AccountDestination = accountDestination;
    public void SetAccountDestinationId(int accountDestinationId) => AccountDestinationId = accountDestinationId;
    public void SetAmount(decimal amount) => Amount = amount;
    public void SetTransferDate(DateTime transferDate) => TransferDate = transferDate;
}