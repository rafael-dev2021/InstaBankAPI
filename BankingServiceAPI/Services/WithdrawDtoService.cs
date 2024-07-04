using AutoMapper;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using BankingServiceAPI.Services.Interfaces;
using Serilog;

namespace BankingServiceAPI.Services;

public class WithdrawDtoService(
    IWithdrawRepository withdrawRepository,
    IBankTransactionRepository bankTransactionRepository,
    ITransactionLogService transactionCreationService,
    IMapper mapper) : IWithdrawDtoService
{
    public async Task<WithdrawDtoResponse> WithdrawDtoAsync(string userId, int accountNumber, decimal amount)
    {
        Log.Information(
            "[WITHDRAW] Attempting to withdraw [{Amount}] from account number [{AccountNumber}] for user [{UserId}]",
            amount, accountNumber, userId);

        var account = await GetAccountAsync(userId, accountNumber);

        ValidateUserAuthorization(userId, account);

        var withdraw = await ExecuteWithdrawAsync(account, amount);

        await SaveWithdrawAsync(withdraw);

        Log.Information(
            "[WITHDRAW] Withdrawal of [{Amount}] from account number [{AccountNumber}] for user [{UserId}] successfully completed",
            withdraw.Amount, withdraw.AccountOrigin!.AccountNumber, userId);

        return mapper.Map<WithdrawDtoResponse>(withdraw);
    }

    private async Task<BankAccount> GetAccountAsync(string userId, int accountNumber)
    {
        var account = await withdrawRepository.GetByAccountNumberAsync(accountNumber);
        if (account == null)
        {
            Log.Warning("[GET_ACCOUNT] Account number [{AccountNumber}] not found for user [{UserId}]", accountNumber,
                userId);
            throw new AccountNotFoundException("Account not found.");
        }

        Log.Information("[GET_ACCOUNT] Bank account found: [{UserName}] [{UserLastName}], proceeding with withdrawal",
            account.User!.Name, account.User!.LastName);

        return account;
    }

    private static void ValidateUserAuthorization(string userId, BankAccount account)
    {
        if (account.User!.Id == userId) return;

        Log.Warning(
            "[VALIDATE_USER_AUTHORIZATION] User [{UserId}] is not authorized to withdraw from account number [{AccountNumber}]",
            userId,
            account.AccountNumber);
        throw new UnauthorizedAccessException("You are not authorized to perform this transaction.");
    }

    private static Task<Withdraw> ExecuteWithdrawAsync(BankAccount account, decimal amount)
    {
        var withdraw = new Withdraw();
        withdraw.SetAccountOrigin(account);
        withdraw.SetAccountOriginId(account.Id);
        withdraw.SetAccountDestination(account);
        withdraw.SetAccountDestinationId(account.Id);
        withdraw.SetAmount(amount);
        withdraw.SetTransferDate(DateTime.Now);

        withdraw.Execute();

        Log.Information(
            "[EXECUTE_WITHDRAW] Withdraw entity created for account number [{AccountNumber}] with amount [{Amount}]",
            account.AccountNumber, amount);

        return Task.FromResult(withdraw);
    }

    private async Task SaveWithdrawAsync(Withdraw withdraw)
    {
        await bankTransactionRepository.CreateEntityAsync(withdraw);

        await CreateAndSaveTransactionLogAsync(withdraw);
        Log.Information(
            "[SAVE_WITHDRAW] Withdrawal of [{Amount}] from account number [{AccountNumber}] successfully completed",
            withdraw.Amount, withdraw.AccountOrigin!.AccountNumber);
    }

    private async Task CreateAndSaveTransactionLogAsync(Withdraw withdraw)
    {
        Log.Information("[CREATE_TRANSACTION_LOG] Creating transaction log for withdraw [{WithdrawId}]", withdraw.Id);

        var transactionDetails = await transactionCreationService.CreateTransactionDetailsAsync(withdraw);
        var transactionAudit = await transactionCreationService.CreateTransactionAuditAsync(withdraw);
        await transactionCreationService.CreateTransactionLogAsync(withdraw, transactionDetails, transactionAudit);

        Log.Information("[CREATE_TRANSACTION_LOG] Transaction log created for withdraw [{WithdrawId}]", withdraw.Id);
    }
}