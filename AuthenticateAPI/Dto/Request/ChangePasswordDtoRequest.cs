namespace AuthenticateAPI.Dto.Request;

public record ChangePasswordDtoRequest(string? Email, string? CurrentPassword, string? NewPassword);