namespace AuthenticateAPI.Dto.Response;

public record UserDtoResponse(string Id, string Name, string LastName, string Email,string Role) { }