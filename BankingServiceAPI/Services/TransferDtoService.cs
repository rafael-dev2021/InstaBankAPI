using AutoMapper;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using BankingServiceAPI.Services.Interfaces;
using Serilog;

namespace BankingServiceAPI.Services;

public class TransferDtoService(
    ITransferRepository transferRepository,
    IBankTransactionRepository bankTransactionRepository,
    ITransactionLogService transactionCreationService,
    IMapper mapper) : ITransferDtoService
{
    public async Task<TransferByBankAccountNumberDtoResponse> TransferByBankAccountNumberDtoAsync(string userId,
        int originAccountNumber, int destinationAccountNumber,
        decimal amount)
    {
        Log.Information(
            "[TRANSFER_BY_ACCOUNT_NUMBER] Attempting to transfer [{Amount}] from account number [{OriginAccountNumber}] to account number [{DestinationAccountNumber}] for user [{UserId}]",
            amount, originAccountNumber, destinationAccountNumber, userId);

        var originAccount = await transferRepository.GetByAccountNumberAsync(originAccountNumber);
        var destinationAccount = await transferRepository.GetByAccountNumberAsync(destinationAccountNumber);

        var transfer = await ExecuteTransferAsync(userId, originAccount, destinationAccount, amount);

        Log.Information(
            "[TRANSFER_BY_ACCOUNT_NUMBER] Transfer of [{Amount}] from account number [{OriginAccountNumber}] to account number [{DestinationAccountNumber}] for user [{UserId}] successfully completed",
            amount, originAccountNumber, destinationAccountNumber, userId);
        return mapper.Map<TransferByBankAccountNumberDtoResponse>(transfer);
    }

    public async Task<TransferByCpfDtoResponse> TransferByCpfDtoAsync(string userId, string? originCpf,
        string? destinationCpf,
        decimal amount)
    {
        Log.Information(
            "[TRANSFER_BY_CPF] Attempting to transfer [{Amount}] from account with CPF [{OriginCpf}] to account with CPF [{DestinationCpf}] for user [{UserId}]",
            amount, originCpf, destinationCpf, userId);

        var originAccount = await transferRepository.GetByCpfAsync(originCpf!);
        var destinationAccount = await transferRepository.GetByCpfAsync(destinationCpf!);

        var transfer = await ExecuteTransferAsync(userId, originAccount, destinationAccount, amount);

        Log.Information(
            "[TRANSFER_BY_CPF] Transfer of [{Amount}] from account with CPF [{OriginCpf}] to account with CPF [{DestinationCpf}] for user [{UserId}] successfully completed",
            amount, originCpf, destinationCpf, userId);
        return mapper.Map<TransferByCpfDtoResponse>(transfer);
    }

    private async Task<Transfer> ExecuteTransferAsync(string? userId, BankAccount? originAccount,
        BankAccount? destinationAccount,
        decimal amount)
    {
        ValidateAccounts(userId, originAccount, destinationAccount);

        var transfer = new Transfer();
        transfer.SetAccountOrigin(originAccount!);
        transfer.SetAccountDestination(destinationAccount!);
        transfer.SetAmount(amount);
        transfer.SetTransferDate(DateTime.UtcNow);

        Log.Information(
            "[EXECUTE_TRANSFER] Transfer entity created for origin account number [{OriginAccountNumber}] to destination account number [{DestinationAccountNumber}] with amount [{Amount}]",
            originAccount!.AccountNumber, destinationAccount!.AccountNumber, amount);

        transfer.Execute();

        await bankTransactionRepository.CreateEntityAsync(transfer);

        await CreateAndSaveTransactionLogAsync(transfer);

        Log.Information(
            "[EXECUTE_TRANSFER] Transfer of [{Amount}] from account number [{OriginAccountNumber}] to account number [{DestinationAccountNumber}] for user [{UserId}] successfully completed",
            amount, originAccount.AccountNumber, destinationAccount.AccountNumber, userId);

        return transfer;
    }

    private static void ValidateAccounts(string? userId, BankAccount? originAccount, BankAccount? destinationAccount)
    {
        if (originAccount == null)
        {
            Log.Warning("[VALIDATE_ACCOUNTS] Origin account not found for user [{UserId}]", userId);
            throw new AccountNotFoundException("Origin account not found.");
        }

        if (destinationAccount == null)
        {
            Log.Warning("[VALIDATE_ACCOUNTS] Destination account not found for user [{UserId}]", userId);
            throw new AccountNotFoundException("Destination account not found.");
        }

        if (originAccount.User == null || originAccount.User.Id != userId)
        {
            throw new UnauthorizedAccessException("User not authorized to transfer from this account.");
        }
    }

    private async Task CreateAndSaveTransactionLogAsync(Transfer transfer)
    {
        Log.Information("[CREATE_TRANSACTION_LOG] Creating transaction log for transfer [{TransferId}]", transfer.Id);

        var transactionDetails = await transactionCreationService.CreateTransactionDetailsAsync(transfer);
        var transactionAudit = await transactionCreationService.CreateTransactionAuditAsync(transfer);
        await transactionCreationService.CreateTransactionLogAsync(transfer, transactionDetails, transactionAudit);

        Log.Information("[CREATE_TRANSACTION_LOG] Transaction log created for transfer [{TransferId}]", transfer.Id);
    }
}