using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticateAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticateAPI.Security;

public class TokenService
{
    private readonly IConfiguration _configuration;
    private readonly string? _secretKey;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? configuration["Jwt:SecretKey"];

        if (string.IsNullOrEmpty(_secretKey))
        {
            throw new Exception("JWT secret key is not set.");
        }
    }

    public string GenerateToken(User user, int expirationToken)
    {
        var key = Encoding.ASCII.GetBytes(_secretKey!);
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
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateAccessToken(User user)
    {
        var expirationToken = int.Parse(_configuration["Jwt:ExpirationTokenMinutes"] ?? string.Empty);
        return GenerateToken(user, expirationToken);
    }

    public string GenerateRefreshToken(User user)
    {
        var expirationToken = int.Parse(_configuration["Jwt:RefreshTokenExpirationMinutes"] ?? string.Empty);
        return GenerateToken(user, expirationToken);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var key = Encoding.ASCII.GetBytes(_secretKey!);
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
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