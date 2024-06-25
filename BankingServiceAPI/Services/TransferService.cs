﻿using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using BankingServiceAPI.Services.Interfaces;

namespace BankingServiceAPI.Services;

public class TransferService(
    ITransferRepository transferRepository,
    IBankTransactionRepository bankTransactionRepository) : ITransferService
{
    public async Task<Transfer> TransferAsync(int originAccountNumber, int destinationAccountNumber, decimal amount)
    {
        var originAccount = await transferRepository.GetByAccountNumberAsync(originAccountNumber);
        var destinationAccount = await transferRepository.GetByAccountNumberAsync(destinationAccountNumber);

        return await ExecuteTransferAsync(originAccount, destinationAccount, amount);
    }

    public async Task<Transfer> TransferByCpfAsync(string? originCpf, string? destinationCpf, decimal amount)
    {
        var originAccount = await transferRepository.GetByCpfAsync(originCpf!);
        var destinationAccount = await transferRepository.GetByCpfAsync(destinationCpf!);

        return await ExecuteTransferAsync(originAccount, destinationAccount, amount);
    }
    
    private async Task<Transfer> ExecuteTransferAsync(BankAccount? originAccount, BankAccount? destinationAccount, decimal amount)
    {
        ValidateAccounts(originAccount, destinationAccount);

        var transfer = new Transfer();
        transfer.SetAccountOrigin(originAccount!);
        transfer.SetAccountDestination(destinationAccount!);
        transfer.SetAmount(amount);
        transfer.SetTransferDate(DateTime.UtcNow);

        transfer.Execute();

        await bankTransactionRepository.CreateEntityAsync(transfer);

        return transfer;
    }
    
    private static void ValidateAccounts(BankAccount? originAccount, BankAccount? destinationAccount)
    {
        if (originAccount == null)
        {
            throw new AccountNotFoundException("Origin account not found.");
        }

        if (destinationAccount == null)
        {
            throw new AccountNotFoundException("Destination account not found.");
        }
    }
}