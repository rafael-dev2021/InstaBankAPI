namespace BankingServiceAPI.Dto.Request;

public record WithdrawDtoRequest(int AccountNumber, decimal Amount);