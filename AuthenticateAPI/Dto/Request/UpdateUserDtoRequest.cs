namespace AuthenticateAPI.Dto.Request;

public record UpdateUserDtoRequest(string? Name, string? LastName, string? Email, string? PhoneNumber){ }