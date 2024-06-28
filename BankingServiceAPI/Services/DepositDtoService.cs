using AutoMapper;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using BankingServiceAPI.Services.Interfaces;

namespace BankingServiceAPI.Services;

public class DepositDtoService(
    IDepositRepository depositRepository,
    IBankTransactionRepository bankTransactionRepository,
    ILogger<DepositDtoService> logger,
    IMapper mapper)
    : IDepositDtoService
{
    public async Task<DepositDtoResponse> DepositDtoAsync(string userId, int accountNumber, decimal amount)
    {
        logger.LogInformation("Attempting to deposit {Amount} to account number {AccountNumber} for user {UserId}",
            amount, accountNumber, userId);

        var account = await GetAccountAsync(userId, accountNumber);

        ValidateUserAuthorization(userId, account);

        var deposit = CreateDeposit(account, amount);

        await SaveDepositAsync(deposit);

        return mapper.Map<DepositDtoResponse>(deposit);
    }

    private async Task<BankAccount> GetAccountAsync(string userId, int accountNumber)
    {
        var account = await depositRepository.GetByAccountNumberAsync(accountNumber);
        if (account == null)
        {
            logger.LogWarning("Account number {AccountNumber} not found for user {UserId}", accountNumber, userId);
            throw new AccountNotFoundException("Account not found.");
        }

        logger.LogInformation("Account found: {Email}, proceeding with deposit", account.User!.Email);

        return account;
    }

    private void ValidateUserAuthorization(string userId, BankAccount account)
    {
        if (account.User != null && account.User.Id == userId) return;

        logger.LogWarning("User {UserId} is not authorized to deposit to account number {AccountNumber}", userId,
            account.AccountNumber);
        throw new UnauthorizedAccessException("User not authorized to deposit to this account.");
    }

    private Deposit CreateDeposit(BankAccount account, decimal amount)
    {
        var deposit = new Deposit();
        deposit.SetAccountOrigin(account);
        deposit.SetAccountOriginId(account.Id);
        deposit.SetAccountDestination(account);
        deposit.SetAccountDestinationId(account.Id);
        deposit.SetAmount(amount);
        deposit.SetTransferDate(DateTime.Now);

        deposit.Execute();

        logger.LogInformation("Deposit entity created for account number {AccountNumber} with amount {Amount}",
            account.AccountNumber, amount);

        return deposit;
    }

    private async Task SaveDepositAsync(Deposit deposit)
    {
        await bankTransactionRepository.CreateEntityAsync(deposit);

        logger.LogInformation(
            "Deposit of {Amount} to account number {AccountNumber} for user {UserId} successfully completed",
            deposit.Amount, deposit.AccountOrigin!.AccountNumber, deposit.AccountOrigin.User!.Id);
    }
}