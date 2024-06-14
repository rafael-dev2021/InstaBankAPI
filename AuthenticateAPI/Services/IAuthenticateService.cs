using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;

namespace AuthenticateAPI.Services;

public interface IAuthenticateService
{
    Task<TokenDtoResponse> LoginAsync(LoginDtoRequest request);
    Task<TokenDtoResponse> RegisterAsync(RegisterDtoRequest request);
    Task<TokenDtoResponse> UpdateAsync(UpdateUserDtoRequest request, string userId);
    Task<bool> ChangePasswordAsync(ChangePasswordDtoRequest request);
}