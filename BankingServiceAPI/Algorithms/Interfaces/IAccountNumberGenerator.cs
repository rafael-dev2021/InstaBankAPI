namespace BankingServiceAPI.Algorithms.Interfaces;

public interface IAccountNumberGenerator
{
    int GenerateAccountNumber();
    int GenerateAgencyNumber();
}