using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using BankingServiceAPI.Services.Interfaces;

namespace BankingServiceAPI.Services;

public class WithdrawService(
    IWithdrawRepository withdrawRepository,
    IBankTransactionRepository bankTransactionRepository,
    ILogger<WithdrawService> logger) : IWithdrawService
{
    public async Task<Withdraw> WithdrawAsync(string userId, int accountNumber, decimal amount)
    {
        logger.LogInformation("Withdraw account called");

        var account = await withdrawRepository.GetByAccountNumberAsync(accountNumber);
        if (account == null)
        {
            throw new AccountNotFoundException("Account not found.");
        }

        if (account.User!.Id != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to perform this transaction.");
        }

        logger.LogInformation("Bank Account found Fullname: {name} {lastname}", account.User!.Name,
            account.User!.LastName);

        var withdraw = new Withdraw();
        withdraw.SetAccountOrigin(account);
        withdraw.SetAccountOriginId(account.Id);
        withdraw.SetAccountDestination(account);
        withdraw.SetAccountDestinationId(account.Id);
        withdraw.SetAmount(amount);
        withdraw.SetTransferDate(DateTime.Now);

        logger.LogInformation("Withdraw created");

        withdraw.Execute();
        logger.LogInformation("Withdraw executed");

        await bankTransactionRepository.CreateEntityAsync(withdraw);

        return withdraw;
    }
}