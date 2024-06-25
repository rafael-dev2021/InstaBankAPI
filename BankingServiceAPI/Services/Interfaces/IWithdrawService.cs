using BankingServiceAPI.Models;

namespace BankingServiceAPI.Services.Interfaces;

public interface IWithdrawService
{
    Task<Withdraw> WithdrawAsync(string userId, int accountNumber, decimal amount);
}