using BankingServiceAPI.Dto.Response;

namespace BankingServiceAPI.Services.Interfaces;

public interface IWithdrawDtoService
{
    Task<WithdrawDtoResponse> WithdrawDtoAsync(string userId, int accountNumber, decimal amount);
}