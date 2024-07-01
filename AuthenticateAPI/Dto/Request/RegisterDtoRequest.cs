namespace AuthenticateAPI.Dto.Request;

public record RegisterDtoRequest(
    string? Name,
    string? LastName,
    string? PhoneNumber,
    string Cpf,
    string? Email,
    string Password,
    string? ConfirmPassword);