namespace AuthenticateAPI.Dto.Request;

public record ChangePasswordDtoRequest(string Email, string OldPassword, string NewPassword);