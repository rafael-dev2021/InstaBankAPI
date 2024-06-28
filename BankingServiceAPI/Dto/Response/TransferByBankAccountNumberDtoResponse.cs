namespace BankingServiceAPI.Dto.Response;

public record TransferByBankAccountNumberDtoResponse(
    int TransactionId,
    string Name,
    string LastName,
    int DestinationBankAccountNumber,
    decimal Amount,
    DateTime TransferDate);