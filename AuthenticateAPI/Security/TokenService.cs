using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticateAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticateAPI.Security;

public class TokenService(JwtSettings jwtSettings, ILogger<TokenService> logger) : ITokenService
{
    public string GenerateToken(User user, int expirationToken)
    {
        logger.LogInformation("Generating token for user: {Email}", user.Email);

        var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey!);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Email!),
                new Claim("Name", user.Name!),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }),
            Expires = DateTime.UtcNow.AddMinutes(expirationToken),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        logger.LogInformation("Token generated successfully for user: {Email}", user.Email);
        return tokenString;
    }

    public string GenerateAccessToken(User user)
    {
        logger.LogInformation("Generating access token for user: {Email}", user.Email);
        return GenerateToken(user, jwtSettings.ExpirationTokenMinutes);
    }

    public string GenerateRefreshToken(User user)
    {
        logger.LogInformation("Generating refresh token for user: {Email}", user.Email);
        return GenerateToken(user, jwtSettings.RefreshTokenExpirationMinutes);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        logger.LogInformation("Validating token");
        var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey!);
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            logger.LogInformation("Token validated successfully");
            return principal;
        }
        catch (Exception ex)
        {
            logger.LogWarning("Token validation failed: {Message}", ex.Message);
            return null;
        }
    }
}