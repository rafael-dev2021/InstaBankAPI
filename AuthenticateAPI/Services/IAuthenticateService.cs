using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;

namespace AuthenticateAPI.Services;

public interface IAuthenticateService
{
    Task<IEnumerable<UserDtoResponse>> GetAllUsersServiceAsync();
    Task<TokenDtoResponse> LoginServiceAsync(LoginDtoRequest request);
    Task<TokenDtoResponse> RegisterServiceAsync(RegisterDtoRequest request);
    Task<TokenDtoResponse> UpdateUserServiceAsync(UpdateUserDtoRequest request, string userId);
    Task<bool> ChangePasswordServiceAsync(ChangePasswordDtoRequest request, string userId);
    Task LogoutServiceAsync();
    Task<bool> ForgotPasswordServiceAsync(string email, string newPassword);
    Task<TokenDtoResponse> RefreshTokenServiceAsync(RefreshTokenDtoRequest refreshTokenDtoRequest); 
    Task<bool> RevokedTokenServiceAsync(string token);
    Task<bool> ExpiredTokenServiceAsync(string token);
}