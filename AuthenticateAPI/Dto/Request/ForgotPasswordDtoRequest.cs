namespace AuthenticateAPI.Dto.Request;

public record ForgotPasswordDtoRequest(string? Email, string? NewPassword);