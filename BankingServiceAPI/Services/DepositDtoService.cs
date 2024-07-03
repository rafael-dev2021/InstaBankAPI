using AutoMapper;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using BankingServiceAPI.Services.Interfaces;
using Serilog;

namespace BankingServiceAPI.Services;

public class DepositDtoService(
    IDepositRepository depositRepository,
    IBankTransactionRepository bankTransactionRepository,
    ITransactionLogService transactionCreationService,
    IMapper mapper)
    : IDepositDtoService
{
    public async Task<DepositDtoResponse> DepositDtoAsync(string userId, int accountNumber, decimal amount)
    {
        Log.Information(
            "[DEPOSIT] Attempting to deposit [{Amount}] to account number [{AccountNumber}] for user [{UserId}]",
            amount, accountNumber, userId);

        var account = await GetAccountAsync(userId, accountNumber);

        ValidateUserAuthorization(userId, account);

        var deposit = await ExecuteDepositAsync(account, amount);

        await SaveDepositAsync(deposit);

        Log.Information(
            "[DEPOSIT] Deposit of [{Amount}] to account number [{AccountNumber}] for user [{UserId}] successfully completed",
            deposit.Amount, deposit.AccountOrigin!.AccountNumber, deposit.AccountOrigin.User!.Id);

        return mapper.Map<DepositDtoResponse>(deposit);
    }

    private async Task<BankAccount> GetAccountAsync(string userId, int accountNumber)
    {
        var account = await depositRepository.GetByAccountNumberAsync(accountNumber);
        if (account == null)
        {
            Log.Warning("[GET_ACCOUNT] Account number [{AccountNumber}] not found for user [{UserId}]", accountNumber,
                userId);
            throw new AccountNotFoundException("Account not found.");
        }

        Log.Information("[GET_ACCOUNT] Account found: [{Email}], proceeding with deposit", account.User!.Email);

        return account;
    }

    private static void ValidateUserAuthorization(string userId, BankAccount account)
    {
        if (account.User != null && account.User.Id == userId) return;

        Log.Warning(
            "[VALIDATE_USER_AUTHORIZATION] User [{UserId}] is not authorized to deposit to account number [{AccountNumber}]",
            userId,
            account.AccountNumber);
        throw new UnauthorizedAccessException("User not authorized to deposit to this account.");
    }

    private static Task<Deposit> ExecuteDepositAsync(BankAccount account, decimal amount)
    {
        var deposit = new Deposit();
        deposit.SetAccountOrigin(account);
        deposit.SetAccountOriginId(account.Id);
        deposit.SetAccountDestination(account);
        deposit.SetAccountDestinationId(account.Id);
        deposit.SetAmount(amount);
        deposit.SetTransferDate(DateTime.Now);

        deposit.Execute();

        Log.Information(
            "[EXECUTE_DEPOSIT] Deposit entity created for account number [{AccountNumber}] with amount [{Amount}]",
            account.AccountNumber, amount);

        return Task.FromResult(deposit);
    }

    private async Task SaveDepositAsync(Deposit deposit)
    {
        await bankTransactionRepository.CreateEntityAsync(deposit);

        await CreateAndSaveTransactionLogAsync(deposit);

        Log.Information(
            "[SAVE_DEPOSIT] Deposit of [{Amount}] to account number [{AccountNumber}] for user [{UserId}] successfully completed",
            deposit.Amount, deposit.AccountOrigin!.AccountNumber, deposit.AccountOrigin.User!.Id);
    }

    private async Task CreateAndSaveTransactionLogAsync(Deposit deposit)
    {
        Log.Information("[CREATE_TRANSACTION_LOG] Creating transaction log for deposit [{DepositId}]", deposit.Id);

        var transactionDetails = await transactionCreationService.CreateTransactionDetailsAsync(deposit);
        var transactionAudit = await transactionCreationService.CreateTransactionAuditAsync(deposit);
        await transactionCreationService.CreateTransactionLogAsync(deposit, transactionDetails, transactionAudit);

        Log.Information("[CREATE_TRANSACTION_LOG] Transaction log created for deposit [{DepositId}]", deposit.Id);
    }
}