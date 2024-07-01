using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;

namespace AuthenticateAPI.Services;

public interface IAuthenticateService
{
    Task<IEnumerable<UserDtoResponse>> GetAllUsersDtoAsync();
    Task<TokenDtoResponse> LoginAsync(LoginDtoRequest request);
    Task<TokenDtoResponse> RegisterAsync(RegisterDtoRequest request);
    Task<TokenDtoResponse> UpdateUserDtoAsync(UpdateUserDtoRequest request, string userId);
    Task<bool> ChangePasswordAsync(ChangePasswordDtoRequest request, string userId);
    Task LogoutAsync();
    Task<bool> ForgotPasswordAsync(string email, string newPassword);
    Task<TokenDtoResponse> RefreshTokenAsync(RefreshTokenDtoRequest refreshTokenDtoRequest); 
    Task<bool> RevokedTokenAsync(string token);
    Task<bool> ExpiredTokenAsync(string token);
}