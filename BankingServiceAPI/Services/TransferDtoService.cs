using AutoMapper;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using BankingServiceAPI.Services.Interfaces;

namespace BankingServiceAPI.Services;

public class TransferDtoService(
    ITransferRepository transferRepository,
    IBankTransactionRepository bankTransactionRepository,
    ILogger<TransferDtoService> logger,
    IMapper mapper) : ITransferDtoService
{
    public async Task<TransferByBankAccountNumberDtoResponse> TransferByBankAccountNumberDtoAsync(string userId,
        int originAccountNumber, int destinationAccountNumber,
        decimal amount)
    {
        logger.LogInformation(
            "Attempting to transfer {Amount} from account number {OriginAccountNumber} to account number {DestinationAccountNumber} for user {UserId}",
            amount, originAccountNumber, destinationAccountNumber, userId);

        var originAccount = await transferRepository.GetByAccountNumberAsync(originAccountNumber);
        var destinationAccount = await transferRepository.GetByAccountNumberAsync(destinationAccountNumber);

        var transfer = await ExecuteTransferAsync(userId, originAccount, destinationAccount, amount);
        
        return mapper.Map<TransferByBankAccountNumberDtoResponse>(transfer);
    }

    public async Task<TransferDtoResponse> TransferByCpfDtoAsync(string userId, string? originCpf, string? destinationCpf,
        decimal amount)
    {
        logger.LogInformation(
            "Attempting to transfer {Amount} from account with CPF {OriginCpf} to account with CPF {DestinationCpf} for user {UserId}",
            amount, originCpf, destinationCpf, userId);

        var originAccount = await transferRepository.GetByCpfAsync(originCpf!);
        var destinationAccount = await transferRepository.GetByCpfAsync(destinationCpf!);

        var transfer = await ExecuteTransferAsync(userId, originAccount, destinationAccount, amount);

        return mapper.Map<TransferDtoResponse>(transfer);
    }

    private async Task<Transfer> ExecuteTransferAsync(string? userId, BankAccount? originAccount,
        BankAccount? destinationAccount,
        decimal amount)
    {
        ValidateAccounts(userId, originAccount, destinationAccount);

        logger.LogInformation("Accounts validated. Proceeding with transfer.");

        var transfer = new Transfer();
        transfer.SetAccountOrigin(originAccount!);
        transfer.SetAccountDestination(destinationAccount!);
        transfer.SetAmount(amount);
        transfer.SetTransferDate(DateTime.UtcNow);

        logger.LogInformation(
            "Transfer entity created for origin account number {OriginAccountNumber} to destination account number {DestinationAccountNumber} with amount {Amount}",
            originAccount!.AccountNumber, destinationAccount!.AccountNumber, amount);

        transfer.Execute();

        await bankTransactionRepository.CreateEntityAsync(transfer);

        logger.LogInformation(
            "Transfer of {Amount} from account number {OriginAccountNumber} to account number {DestinationAccountNumber} for user {UserId} successfully completed",
            amount, originAccount.AccountNumber, destinationAccount.AccountNumber, userId);

        return transfer;
    }

    private void ValidateAccounts(string? userId, BankAccount? originAccount, BankAccount? destinationAccount)
    {
        if (originAccount == null)
        {
            logger.LogWarning("Origin account not found for user {UserId}", userId);
            throw new AccountNotFoundException("Origin account not found.");
        }

        if (destinationAccount == null)
        {
            logger.LogWarning("Destination account not found for user {UserId}", userId);
            throw new AccountNotFoundException("Destination account not found.");
        }

        if (originAccount.User == null || originAccount.User.Id != userId)
        {
            throw new UnauthorizedAccessException("User not authorized to transfer from this account.");
        }
    }
}