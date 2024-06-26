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
        logger.LogInformation("Attempting to withdraw {Amount} from account number {AccountNumber} for user {UserId}", amount, accountNumber, userId);

        var account = await withdrawRepository.GetByAccountNumberAsync(accountNumber);
        if (account == null)
        {
            logger.LogWarning("Account number {AccountNumber} not found for user {UserId}", accountNumber, userId);
            throw new AccountNotFoundException("Account not found.");
        }

        if (account.User!.Id != userId)
        {
            logger.LogWarning("User {UserId} is not authorized to withdraw from account number {AccountNumber}", userId, accountNumber);
            throw new UnauthorizedAccessException("You are not authorized to perform this transaction.");
        }

        logger.LogInformation("Bank account found: {UserName} {UserLastName}, proceeding with withdrawal", account.User!.Name, account.User!.LastName);

        var withdraw = new Withdraw();
        withdraw.SetAccountOrigin(account);
        withdraw.SetAccountOriginId(account.Id);
        withdraw.SetAccountDestination(account);
        withdraw.SetAccountDestinationId(account.Id);
        withdraw.SetAmount(amount);
        withdraw.SetTransferDate(DateTime.Now);

        logger.LogInformation("Withdraw entity created for account number {AccountNumber} with amount {Amount}", accountNumber, amount);

        withdraw.Execute();

        await bankTransactionRepository.CreateEntityAsync(withdraw);

        logger.LogInformation("Withdrawal of {Amount} from account number {AccountNumber} for user {UserId} successfully completed", amount, accountNumber, userId);

        return withdraw;
    }
}