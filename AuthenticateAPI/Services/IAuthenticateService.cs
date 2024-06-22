using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;

namespace AuthenticateAPI.Services;

public interface IAuthenticateService
{
    Task<IEnumerable<UserDtoResponse>> GetAllUsersDtoAsync();
    Task<TokenDtoResponse> LoginAsync(LoginDtoRequest request);
    Task<TokenDtoResponse> RegisterAsync(RegisterDtoRequest request);
    Task<TokenDtoResponse> UpdateAsync(UpdateUserDtoRequest request, string userId);
    Task<bool> ChangePasswordAsync(ChangePasswordDtoRequest request);
    Task LogoutAsync();
    Task<bool> ForgotPasswordAsync(string email, string newPassword);
    Task<TokenDtoResponse> RefreshTokenAsync(RefreshTokenDtoRequest refreshTokenDtoRequest); // Adicione esta linha
}