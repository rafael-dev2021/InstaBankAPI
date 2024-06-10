using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;

namespace AuthenticateAPI.Repositories;

public interface IAuthenticatedRepository
{
    Task<AuthenticatedDtoResponse> AuthenticateAsync(LoginDtoRequest loginDtoRequest);
    Task<RegisteredDtoResponse> RegisterAsync(User user);
    Task<(bool success, string errorMessage)> UpdateProfileAsync(UpdateUserDtoRequest updateUserDtoRequest);
    Task<bool> ChangePasswordAsync(ChangePasswordDtoRequest changePasswordDtoRequest);
    Task<User> GetUserProfileAsync(string userEmail);
    Task<bool> ForgotPasswordAsync(string email, string newPassWord);
    Task LogoutAsync();
}