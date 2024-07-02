using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticateAPI.Models;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace AuthenticateAPI.Security;
 
public class TokenService(JwtSettings jwtSettings) : ITokenService
{
    public string GenerateToken(User user, int expirationToken)
    {
        Log.Information("[GENERATE_TOKEN] Generating token for user: [{Email}]", user.Email);

        var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey!);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Email!),
                new Claim(ClaimTypes.GivenName, user.Name!),
                new Claim(ClaimTypes.Surname, user.LastName!),
                new Claim("Cpf", user.Cpf),
                new Claim("PhoneNumber", user.PhoneNumber!),
                new Claim(ClaimTypes.Email, user.Email!), 
                new Claim(ClaimTypes.Role, user.Role!),
            }),
            Expires = DateTime.UtcNow.AddMinutes(expirationToken),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        Log.Information("[GENERATE_TOKEN] Token generated successfully for user: [{Email}]", user.Email);
        return tokenString;
    }

    public string GenerateAccessToken(User user)
    {
        Log.Information("[ACCESS_TOKEN] Generating access token for user: [{Email}]", user.Email);
        return GenerateToken(user, jwtSettings.ExpirationTokenMinutes);
    }

    public string GenerateRefreshToken(User user)
    {
        Log.Information("[REFRESH_TOKEN] Generating refresh token for user: [{Email}]", user.Email);
        return GenerateToken(user, jwtSettings.RefreshTokenExpirationMinutes);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        Log.Information("[VALID_TOKEN] Validating token");
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
            Log.Information("[VALID_TOKEN] Token validated successfully");
            return principal;
        }
        catch (Exception ex)
        {
            Log.Error(ex,"[VALID_TOKEN] Token validation failed: {Message}", ex.Message);
            return null;
        }
    }
}