using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticateAPI.Models;
using AuthenticateAPI.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace XUnitTests.AuthenticateAPI.Security;

public class TokenServiceTests
{
    private readonly JwtSettings _jwtSettings;

    protected TokenServiceTests()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json");

        var configuration = configurationBuilder.Build();
        _jwtSettings = new JwtSettings();
        configuration.Bind("Jwt", _jwtSettings);
    }

    public class GenerateTokenTests : TokenServiceTests
    {
        [Fact]
        public void GenerateToken_ShouldReturnValidToken()
        {
            // Arrange
            var tokenService = new TokenService(_jwtSettings);

            var user = new User
            {
                Id = "123",
                Email = "test@example.com",
            };
            user.SetName("Test User");

            // Act
            var token = tokenService.GenerateToken(user, 30);

            // Assert
            Assert.NotNull(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.SecretKey!))
            }, out var validatedToken);

            Assert.NotNull(validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;
            Assert.NotNull(jwtToken);
            Assert.Equal("test@example.com", principal.FindFirst(ClaimTypes.Name)?.Value);
            Assert.Equal("Test User", principal.FindFirst("Name")?.Value);
            Assert.Equal("123", principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }

    public class GenerateAccessTokenTests : TokenServiceTests
    {
        [Fact]
        public void GenerateAccessToken_ShouldReturnValidToken()
        {
            // Arrange
            var tokenService = new TokenService(_jwtSettings);

            var user = new User
            {
                Id = "123",
                Email = "test@example.com",
            };
            user.SetName("Test User");

            // Act
            var token = tokenService.GenerateAccessToken(user);

            // Assert
            Assert.NotNull(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.SecretKey!))
            }, out var validatedToken);

            Assert.NotNull(validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;
            Assert.NotNull(jwtToken);
            Assert.Equal("test@example.com", principal.FindFirst(ClaimTypes.Name)?.Value);
            Assert.Equal("Test User", principal.FindFirst("Name")?.Value);
            Assert.Equal("123", principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }

    public class GenerateRefreshTokenTests : TokenServiceTests
    {
        [Fact]
        public void GenerateRefreshToken_ShouldReturnValidToken()
        {
            // Arrange
            var tokenService = new TokenService(_jwtSettings);

            var user = new User
            {
                Id = "123",
                Email = "test@example.com",
            };
            user.SetName("Test User");

            // Act
            var token = tokenService.GenerateRefreshToken(user);

            // Assert
            Assert.NotNull(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.SecretKey!))
            }, out var validatedToken);

            Assert.NotNull(validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;
            Assert.NotNull(jwtToken);
            Assert.Equal("test@example.com", principal.FindFirst(ClaimTypes.Name)?.Value);
            Assert.Equal("Test User", principal.FindFirst("Name")?.Value);
            Assert.Equal("123", principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }

    public class ValidateTokenTests : TokenServiceTests
    {
        [Fact]
        public void ValidateToken_ShouldReturnPrincipalForValidToken()
        {
            // Arrange
            var tokenService = new TokenService(_jwtSettings);

            var user = new User
            {
                Id = "123",
                Email = "test@example.com",
            };
            user.SetName("Test User");

            var token = tokenService.GenerateAccessToken(user);

            // Act
            var principal = tokenService.ValidateToken(token);

            // Assert
            Assert.NotNull(principal);
            Assert.Equal(user.Email, principal.FindFirst(ClaimTypes.Name)?.Value);
            Assert.Equal(user.Name, principal.FindFirst("Name")?.Value);
            Assert.Equal(user.Id, principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        [Fact]
        public void ValidateToken_ShouldReturnNullForInvalidToken()
        {
            // Arrange
            var tokenService = new TokenService(_jwtSettings);
            const string invalidToken = "invalid_token_string";

            // Act
            var principal = tokenService.ValidateToken(invalidToken);

            // Assert
            Assert.Null(principal);
        }
    }
}