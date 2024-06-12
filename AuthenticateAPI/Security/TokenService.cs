using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticateAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticateAPI.Security;

public class TokenService(IConfiguration configuration)
{
    public string GenerateToken(User user, int expirationToken)
    {
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:SecretKey"] ?? string.Empty);
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
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateAccessToken(User user)
    {
        var expirationToken = int.Parse(configuration["Jwt:ExpirationTokenMinutes"] ?? string.Empty);
        return GenerateToken(user, expirationToken);
    }
    
    public string GenerateRefreshToken(User user)
    {
        var expirationToken = int.Parse(configuration["Jwt:RefreshTokenExpirationMinutes"] ?? string.Empty);
        return GenerateToken(user, expirationToken);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"] ?? string.Empty);
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}