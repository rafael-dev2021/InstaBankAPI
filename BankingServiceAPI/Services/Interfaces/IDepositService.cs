using BankingServiceAPI.Models;

namespace BankingServiceAPI.Services.Interfaces;

public interface IDepositService
{
    Task<Deposit> DepositAsync(string userId, int accountNumber, decimal amount);
}