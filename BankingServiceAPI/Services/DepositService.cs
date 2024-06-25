using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using BankingServiceAPI.Services.Interfaces;

namespace BankingServiceAPI.Services;

public class DepositService(
    IDepositRepository depositRepository,
    IBankTransactionRepository bankTransactionRepository,
    ILogger<DepositService> logger)
    : IDepositService
{
    public async Task<Deposit> DepositAsync(string userId, int accountNumber, decimal amount)
    {
        logger.LogInformation("Deposit account called");

        var account = await depositRepository.GetByAccountNumberAsync(accountNumber);
        if (account == null)
        {
            throw new AccountNotFoundException("Account not found.");
        }

        if (account.User == null || account.User.Id != userId)
        {
            throw new UnauthorizedAccessException("User not authorized to deposit to this account.");
        }

        logger.LogInformation("Account found: {account}", account.User!.Name);

        var deposit = new Deposit();
        deposit.SetAccountOrigin(account);
        deposit.SetAccountOriginId(account.Id);
        deposit.SetAccountDestination(account);
        deposit.SetAccountDestinationId(account.Id);
        deposit.SetAmount(amount);
        deposit.SetTransferDate(DateTime.Now);
        logger.LogInformation("Deposit created");

        deposit.Execute();
        logger.LogInformation("Deposit executed");

        await bankTransactionRepository.CreateEntityAsync(deposit);

        return deposit;
    }
}