namespace BankingServiceAPI.Dto.Response;

public record TransferByCpfDtoResponse(
    int TransactionId,
    string Name,
    string LastName,
    string DestinationCpf,
    decimal Amount,
    DateTime TransferDate);