namespace BankingServiceAPI.Models;

public class BaseEntity
{
    public Guid Id { get; private set; }
    public string? AccountNumber { get; private set; }
    public string? Agency { get; private set; }
    public decimal Balance { get; private set; }

    public void SetId(Guid id) => Id = id;
    public void SetAccountNumber(string? accountNumber) => AccountNumber = accountNumber;
    public void SetAgency(string? agency) => Agency = agency;
    public void SetBalance(decimal balance) => Balance = balance;
}