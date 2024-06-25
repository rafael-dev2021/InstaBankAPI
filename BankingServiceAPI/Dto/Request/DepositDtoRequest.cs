namespace BankingServiceAPI.Dto.Request;

public record DepositDtoRequest(int AccountNumber, decimal Amount);