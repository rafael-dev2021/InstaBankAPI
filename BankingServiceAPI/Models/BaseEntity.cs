namespace BankingServiceAPI.Models;

public class BaseEntity
{
    public Guid Id { get; private set; }
    public string? AccountNumber { get; private set; }
    public string? Agency { get; private set; }
    public decimal Balance { get; private set; }
    public Address? Address { get; private set; } 
    public int AddressId { get; private set; } 

    public void SetId(Guid id) => Id = id;
    public void SetAccountNumber(string? accountNumber) => AccountNumber = accountNumber;
    public void SetAgency(string? agency) => Agency = agency;
    public void SetBalance(decimal balance) => Balance = balance;
    public void SetAddress(Address? address) => Address = address;
    public void SetAddressId(int addressId) => AddressId = addressId;
}