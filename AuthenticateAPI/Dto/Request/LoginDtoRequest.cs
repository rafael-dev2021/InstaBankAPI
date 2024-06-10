namespace AuthenticateAPI.Dto.Request;

public record LoginDtoRequest(string Email, string Password, bool RememberMe);