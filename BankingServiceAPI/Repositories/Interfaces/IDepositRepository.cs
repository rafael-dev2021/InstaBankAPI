using BankingServiceAPI.Models;

namespace BankingServiceAPI.Repositories.Interfaces;

public interface IDepositRepository
{
    Task<BankAccount?> GetByAccountNumberAsync(int accountNumber);
}