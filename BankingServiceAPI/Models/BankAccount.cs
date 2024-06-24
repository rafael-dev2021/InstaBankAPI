namespace BankingServiceAPI.Models;

public class BankAccount
{
    public int Id { get; private set; }
    public int AccountNumber { get; private set; }
    public decimal Balance { get; private set; }
    public int Agency { get; private set; }
    public AccountType AccountType { get; private set; }
    public User? User { get; private set; }

    public void SetId(int id) => Id = id;
    public void SetAccountNumber(int accountNumber) => AccountNumber = accountNumber;
    public void SetBalance(decimal balance) => Balance = balance;
    public void SetAgency(int agency) => Agency = agency;
    public void SetAccountType(AccountType accountType) => AccountType = accountType;
    public void SetUser(User? user) => User = user;
}