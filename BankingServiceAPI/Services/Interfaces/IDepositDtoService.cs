using BankingServiceAPI.Dto.Response;

namespace BankingServiceAPI.Services.Interfaces;

public interface IDepositDtoService
{
    Task<DepositDtoResponse> DepositDtoAsync(string userId, int accountNumber, decimal amount);
}