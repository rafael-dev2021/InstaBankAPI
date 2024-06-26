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
        logger.LogInformation("Attempting to deposit {Amount} to account number {AccountNumber} for user {UserId}",
            amount, accountNumber, userId);

        var account = await depositRepository.GetByAccountNumberAsync(accountNumber);
        if (account == null)
        {
            logger.LogWarning("Account number {AccountNumber} not found for user {UserId}", accountNumber, userId);
            throw new AccountNotFoundException("Account not found.");
        }

        if (account.User == null || account.User.Id != userId)
        {
            logger.LogWarning("User {UserId} is not authorized to deposit to account number {AccountNumber}", userId,
                accountNumber);
            throw new UnauthorizedAccessException("User not authorized to deposit to this account.");
        }

        logger.LogInformation("Account found: {Email}, proceeding with deposit", account.User!.Email);

        var deposit = new Deposit();
        deposit.SetAccountOrigin(account);
        deposit.SetAccountOriginId(account.Id);
        deposit.SetAccountDestination(account);
        deposit.SetAccountDestinationId(account.Id);
        deposit.SetAmount(amount);
        deposit.SetTransferDate(DateTime.Now);

        logger.LogInformation("Deposit entity created for account number {AccountNumber} with amount {Amount}",
            accountNumber, amount);

        deposit.Execute();

        await bankTransactionRepository.CreateEntityAsync(deposit);

        logger.LogInformation(
            "Deposit of {Amount} to account number {AccountNumber} for user {UserId} successfully completed", amount,
            accountNumber, userId);

        return deposit;
    }
}