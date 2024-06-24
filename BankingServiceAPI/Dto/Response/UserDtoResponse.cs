namespace BankingServiceAPI.Dto.Response;

public record UserDtoResponse(
    string? Id,
    string? Name,
    string? LastName,
    string? Cpf,
    string? Email,
    string? PhoneNumber,
    string? Role);