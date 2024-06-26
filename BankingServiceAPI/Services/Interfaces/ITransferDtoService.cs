using BankingServiceAPI.Dto.Response;

namespace BankingServiceAPI.Services.Interfaces;

public interface ITransferDtoService
{
    Task<TransferByBankAccountNumberDtoResponse> TransferByBankAccountNumberDtoAsync(string userId, int originAccountNumber, int destinationAccountNumber, decimal amount);
    Task<TransferDtoResponse> TransferByCpfDtoAsync(string userId, string? originCpf, string? destinationCpf, decimal amount);
}