namespace BankingServiceAPI.Dto.Response;

public record BankAccountDtoResponse(
    int Id,
    int AccountNumber,
    decimal Balance,
    int Agency,
    string AccountType,
    UserDtoResponse? UserDtoResponse);