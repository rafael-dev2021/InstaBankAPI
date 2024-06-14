using System.Security.Claims;
using AuthenticateAPI.Models;

namespace AuthenticateAPI.Security;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken(User user);
    ClaimsPrincipal? ValidateToken(string token);
}