namespace BankingServiceAPI.Dto.Response;

public record WithdrawDtoResponse(
    int TransactionId,
    string Name,
    string LastName,
    string Cpf,
    int BankAccountNumber,
    decimal Amount,
    DateTime TransferDate);
