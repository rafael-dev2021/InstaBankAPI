namespace AuthenticateAPI.Dto.Response;

public record AuthenticatedDtoResponse(bool IsAuthenticated, string ErrorMessage) { }