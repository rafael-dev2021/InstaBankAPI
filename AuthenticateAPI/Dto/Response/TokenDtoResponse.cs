namespace AuthenticateAPI.Dto.Response;

public record TokenDtoResponse(string token, string refreshToken) { }