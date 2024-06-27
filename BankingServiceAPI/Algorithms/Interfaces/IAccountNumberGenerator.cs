namespace BankingServiceAPI.Algorithms.Interfaces;

public interface IAccountNumberGenerator
{
    Task<int> GenerateAccountNumberAsync();
    Task<int> GenerateAgencyNumberAsync();
}