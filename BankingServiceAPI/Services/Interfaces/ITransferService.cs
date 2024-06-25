using BankingServiceAPI.Models;

namespace BankingServiceAPI.Services.Interfaces;

public interface ITransferService
{
    Task<Transfer> TransferAsync(int originAccountNumber, int destinationAccountNumber, decimal amount);
    Task<Transfer> TransferByCpfAsync(string? originCpf, string? destinationCpf, decimal amount);
}