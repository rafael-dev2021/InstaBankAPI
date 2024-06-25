using BankingServiceAPI.Models;

namespace BankingServiceAPI.Repositories.Interfaces;

public interface IWithdrawRepository
{
    Task<BankAccount?> GetByAccountNumberAsync(int accountNumber);
}