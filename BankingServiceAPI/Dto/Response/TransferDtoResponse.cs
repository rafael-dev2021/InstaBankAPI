namespace BankingServiceAPI.Dto.Response;

public record TransferDtoResponse(
    int TransactionId,
    string Name,
    string LastName,
    string DestinationCpf,
    decimal Amount,
    DateTime TransferDate);