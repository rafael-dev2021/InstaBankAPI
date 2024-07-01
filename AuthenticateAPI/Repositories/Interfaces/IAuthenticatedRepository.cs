using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;

namespace AuthenticateAPI.Repositories.Interfaces;

public interface IAuthenticatedRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<AuthenticatedDtoResponse> AuthenticateAsync(LoginDtoRequest loginDtoRequest);
    Task<RegisteredDtoResponse> RegisterAsync(RegisterDtoRequest request);
    Task<UpdatedDtoResponse> UpdateProfileAsync(UpdateUserDtoRequest updateUserDtoRequest, string userId);
    Task<bool> ChangePasswordAsync(ChangePasswordDtoRequest changePasswordDtoRequest);
    Task<User?> GetUserProfileAsync(string userEmail);
    Task<User?> GetUserIdProfileAsync(string userId);
    Task<bool> ForgotPasswordAsync(string email, string newPassword);
    Task LogoutAsync();
    Task SaveAsync(User user);
}