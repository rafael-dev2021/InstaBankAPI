namespace BankingServiceAPI.Dto.Request;

public record TransferDtoRequestByAccount(int OriginAccountNumber, int DestinationAccountNumber, decimal Amount);