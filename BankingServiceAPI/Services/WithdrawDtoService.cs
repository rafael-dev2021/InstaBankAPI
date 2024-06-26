using AutoMapper;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using BankingServiceAPI.Services.Interfaces;

namespace BankingServiceAPI.Services;

public class WithdrawDtoService(
    IWithdrawRepository withdrawRepository,
    IBankTransactionRepository bankTransactionRepository,
    ILogger<WithdrawDtoService> logger,
    IMapper mapper) : IWithdrawDtoService
{
    public async Task<WithdrawDtoResponse> WithdrawDtoAsync(string userId, int accountNumber, decimal amount)
    {
        logger.LogInformation("Attempting to withdraw {Amount} from account number {AccountNumber} for user {UserId}",
            amount, accountNumber, userId);

        var account = await GetAccountAsync(userId, accountNumber);

        ValidateUserAuthorization(userId, account);

        var withdraw = CreateWithdraw(account, amount);

        await SaveWithdrawAsync(withdraw);

        return mapper.Map<WithdrawDtoResponse>(withdraw);
    }
    
    private async Task<BankAccount> GetAccountAsync(string userId, int accountNumber)
    {
        var account = await withdrawRepository.GetByAccountNumberAsync(accountNumber);
        if (account == null)
        {
            logger.LogWarning("Account number {AccountNumber} not found for user {UserId}", accountNumber, userId);
            throw new AccountNotFoundException("Account not found.");
        }

        logger.LogInformation("Bank account found: {UserName} {UserLastName}, proceeding with withdrawal",
            account.User!.Name, account.User!.LastName);

        return account;
    }
    
    private void ValidateUserAuthorization(string userId, BankAccount account)
    {
        if (account.User!.Id == userId) return;
        
        logger.LogWarning("User {UserId} is not authorized to withdraw from account number {AccountNumber}", userId,
            account.AccountNumber);
        throw new UnauthorizedAccessException("You are not authorized to perform this transaction.");
    }

    private Withdraw CreateWithdraw(BankAccount account, decimal amount)
    {
        var withdraw = new Withdraw();
        withdraw.SetAccountOrigin(account);
        withdraw.SetAccountOriginId(account.Id);
        withdraw.SetAccountDestination(account);
        withdraw.SetAccountDestinationId(account.Id);
        withdraw.SetAmount(amount);
        withdraw.SetTransferDate(DateTime.Now);

        withdraw.Execute();

        logger.LogInformation("Withdraw entity created for account number {AccountNumber} with amount {Amount}",
            account.AccountNumber, amount);

        return withdraw;
    }

    private async Task SaveWithdrawAsync(Withdraw withdraw)
    {
        await bankTransactionRepository.CreateEntityAsync(withdraw);

        logger.LogInformation(
            "Withdrawal of {Amount} from account number {AccountNumber} successfully completed",
            withdraw.Amount, withdraw.AccountOrigin!.AccountNumber);
    }
}