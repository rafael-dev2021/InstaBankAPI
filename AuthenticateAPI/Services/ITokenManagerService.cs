using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;

namespace AuthenticateAPI.Services;

public interface ITokenManagerService
{
    Task<TokenDtoResponse> GenerateTokenResponseAsync(User user);
    void RevokeAllUserTokens(User user); 
    Task<bool> RevokedTokenAsync(string token);
    Task<bool> ExpiredTokenAsync(string token);
}