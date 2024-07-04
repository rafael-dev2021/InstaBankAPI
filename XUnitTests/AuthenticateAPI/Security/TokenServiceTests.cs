using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticateAPI.Models;
using AuthenticateAPI.Security;
using Microsoft.IdentityModel.Tokens;

namespace XUnitTests.AuthenticateAPI.Security;

public class TokenServiceTests
{
    private readonly JwtSettings _jwtSettings;

    protected TokenServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            SecretKey = GenerateKey.GenerateHmac256Key(),
            Issuer = "http://localhost",
            Audience = "http://localhost",
            ExpirationTokenMinutes = 15,
            RefreshTokenExpirationMinutes = 30
        };
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
            user.SetLastName("Test User");
            user.SetCpf("123.456.789-10");
            user.SetPhoneNumber("+5540028922");
            user.SetRole("user");

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
            
            Assert.Equal("123", principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Assert.Equal("test@example.com", principal.FindFirst(ClaimTypes.Name)?.Value);
            Assert.Equal("Test User", principal.FindFirst(ClaimTypes.GivenName)?.Value);
            Assert.Equal("Test User", principal.FindFirst(ClaimTypes.Surname)?.Value);
            Assert.Equal("123.456.789-10", principal.FindFirst("Cpf")?.Value);
            Assert.Equal("+5540028922", principal.FindFirst("PhoneNumber")?.Value);
            Assert.Equal("user", principal.FindFirst(ClaimTypes.Role)?.Value);
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
            user.SetLastName("Test User");
            user.SetCpf("123.456.789-10");
            user.SetPhoneNumber("+5540028922");
            user.SetRole("user");

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
            Assert.Equal("123", principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Assert.Equal("test@example.com", principal.FindFirst(ClaimTypes.Name)?.Value);
            Assert.Equal("Test User", principal.FindFirst(ClaimTypes.GivenName)?.Value);
            Assert.Equal("Test User", principal.FindFirst(ClaimTypes.Surname)?.Value);
            Assert.Equal("123.456.789-10", principal.FindFirst("Cpf")?.Value);
            Assert.Equal("+5540028922", principal.FindFirst("PhoneNumber")?.Value);
            Assert.Equal("user", principal.FindFirst(ClaimTypes.Role)?.Value);
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
            user.SetLastName("Test User");
            user.SetCpf("123.456.789-10");
            user.SetPhoneNumber("+5540028922");
            user.SetRole("user");

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
            Assert.Equal("123", principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Assert.Equal("test@example.com", principal.FindFirst(ClaimTypes.Name)?.Value);
            Assert.Equal("Test User", principal.FindFirst(ClaimTypes.GivenName)?.Value);
            Assert.Equal("Test User", principal.FindFirst(ClaimTypes.Surname)?.Value);
            Assert.Equal("123.456.789-10", principal.FindFirst("Cpf")?.Value);
            Assert.Equal("+5540028922", principal.FindFirst("PhoneNumber")?.Value);
            Assert.Equal("user", principal.FindFirst(ClaimTypes.Role)?.Value);
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
            user.SetLastName("Test User");
            user.SetCpf("123.456.789-10");
            user.SetPhoneNumber("+5540028922");
            user.SetRole("user");

            var token = tokenService.GenerateAccessToken(user);

            // Act
            var principal = tokenService.ValidateToken(token);

            // Assert
            Assert.NotNull(principal);
            Assert.Equal("123", principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Assert.Equal("test@example.com", principal.FindFirst(ClaimTypes.Name)?.Value);
            Assert.Equal("Test User", principal.FindFirst(ClaimTypes.GivenName)?.Value);
            Assert.Equal("Test User", principal.FindFirst(ClaimTypes.Surname)?.Value);
            Assert.Equal("123.456.789-10", principal.FindFirst("Cpf")?.Value);
            Assert.Equal("+5540028922", principal.FindFirst("PhoneNumber")?.Value);
            Assert.Equal("user", principal.FindFirst(ClaimTypes.Role)?.Value);
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