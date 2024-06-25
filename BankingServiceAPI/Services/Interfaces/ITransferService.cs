using BankingServiceAPI.Models;

namespace BankingServiceAPI.Services.Interfaces;

public interface ITransferService
{
    Task<Transfer> TransferAsync(string userId, int originAccountNumber, int destinationAccountNumber, decimal amount);
    Task<Transfer> TransferByCpfAsync(string userId, string? originCpf, string? destinationCpf, decimal amount);
}