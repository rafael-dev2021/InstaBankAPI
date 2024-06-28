using BankingServiceAPI.Models;

namespace BankingServiceAPI.Repositories.Interfaces;

public interface IAccountNumberRepository
{
    Task<BankAccount?> GetByAccountNumberAsync(int accountNumber);
}