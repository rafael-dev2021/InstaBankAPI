using System.Security.Claims;
using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using AuthenticateAPI.Security;
using AuthenticateAPI.Services;
using AutoMapper;
using Moq;

namespace XUnitTests.AuthenticateAPI.Services;

public class AuthenticateServiceTests
{
    private readonly Mock<IAuthenticatedRepository> _repositoryMock;
    private readonly AuthenticateService _authenticateService;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ITokenManagerService> _tokenManagerServiceMock;


    protected AuthenticateServiceTests()
    {
        _repositoryMock = new Mock<IAuthenticatedRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _mapperMock = new Mock<IMapper>();
        _tokenManagerServiceMock = new Mock<ITokenManagerService>();
        _authenticateService = new AuthenticateService(
            _repositoryMock.Object,
            _tokenServiceMock.Object,
            _mapperMock.Object,
            _tokenManagerServiceMock.Object
        );
    }

    public class GetAllUsersDtoAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task GetAllUsersDtoAsync_ShouldReturnMappedUserDtos_WhenUsersExist()
        {
            // Arrange
            var user1 = new User { Id = "1", Email = "user1@example.com" };
            user1.SetName("Name 1");
            user1.SetLastName("Last Name 1");
            user1.SetRole("Admin");

            var user2 = new User { Id = "2", Email = "user2@example.com" };
            user2.SetName("Name 2");
            user2.SetLastName("Last Name 2");
            user2.SetRole("User");

            var users = new List<User>
            {
                user1,
                user2
            };

            var userDtos = new List<UserDtoResponse>
            {
                new("1", "Name 1", "Last Name 1", "user1@example.com", "Admin"),
                new("2", "Name 2", "Last Name 2", "user2@example.com", "User")
            };

            _repositoryMock.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserDtoResponse>>(users)).Returns(userDtos);

            // Act
            var result = await _authenticateService.GetAllUsersServiceAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userDtos, result);
        }

        [Fact]
        public async Task GetAllUsersDtoAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Arrange
            var users = new List<User>();

            _repositoryMock.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserDtoResponse>>(users)).Returns(new List<UserDtoResponse>());

            // Act
            var result = await _authenticateService.GetAllUsersServiceAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }

    public class LoginAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task LoginAsync_ShouldReturnTokenDtoResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var loginRequest = new LoginDtoRequest("test@example.com", "password", true);
            var authResponse = new AuthenticatedDtoResponse(true, "Success");
            var user = new User { Email = "test@example.com", Id = "1" };
            user.SetName("Test");
            user.SetLastName("Test");
            user.SetCpf("131.515.555-12");
            var tokenResponse = new TokenDtoResponse("accessToken", "refreshToken");

            _repositoryMock.Setup(r => r.AuthenticateAsync(It.IsAny<LoginDtoRequest>()))
                .ReturnsAsync(authResponse);
            _repositoryMock.Setup(r => r.GetUserProfileAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _tokenManagerServiceMock.Setup(tms => tms.GenerateTokenResponseAsync(It.IsAny<User>()))
                .ReturnsAsync(tokenResponse);

            // Act
            var result = await _authenticateService.LoginServiceAsync(loginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tokenResponse.Token, result.Token);
            Assert.Equal(tokenResponse.RefreshToken, result.RefreshToken);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorizedAccessException_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginRequest = new LoginDtoRequest("test@example.com", "wrongpassword", true);
            var authResponse = new AuthenticatedDtoResponse(false, "Invalid credentials");

            _repositoryMock.Setup(r => r.AuthenticateAsync(It.IsAny<LoginDtoRequest>()))
                .ReturnsAsync(authResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authenticateService.LoginServiceAsync(loginRequest)
            );
            Assert.Equal("Invalid credentials", exception.Message);
        }
    }

    public class RegisterAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task RegisterAsync_ShouldReturnTokenDtoResponse_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var registerRequest = new RegisterDtoRequest(
                "Test", "Test", "1234567890", "131.515.555-12", "test@example.com", "password",
                "password");
            var registerResponse = new RegisteredDtoResponse(true, "Registration successful.");
            var user = new User { Email = "test@example.com", Id = "1" };
            user.SetName("Test");
            user.SetLastName("Test");
            user.SetCpf("131.515.555-12");
            var tokenResponse = new TokenDtoResponse("accessToken", "refreshToken");

            _repositoryMock.Setup(r => r.RegisterAsync(It.IsAny<RegisterDtoRequest>()))
                .ReturnsAsync(registerResponse);
            _repositoryMock.Setup(r => r.GetUserProfileAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _tokenManagerServiceMock.Setup(tms => tms.GenerateTokenResponseAsync(It.IsAny<User>()))
                .ReturnsAsync(tokenResponse);

            // Act
            var result = await _authenticateService.RegisterServiceAsync(registerRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tokenResponse.Token, result.Token);
            Assert.Equal(tokenResponse.RefreshToken, result.RefreshToken);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowUnauthorizedAccessException_WhenRegistrationFails()
        {
            // Arrange
            var registerRequest = new RegisterDtoRequest(
                "Test", "Test", "1234567890", "131.515.555-12", "test@example.com", "password",
                "password");
            var registerResponse = new RegisteredDtoResponse(false, "Registration failed.");

            _repositoryMock.Setup(r => r.RegisterAsync(It.IsAny<RegisterDtoRequest>()))
                .ReturnsAsync(registerResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authenticateService.RegisterServiceAsync(registerRequest)
            );
            Assert.Equal("Registration failed.", exception.Message);
        }
    }

    public class UpdateAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task UpdateAsync_ShouldReturnTokenDtoResponse_WhenUpdateIsSuccessful()
        {
            // Arrange
            var updateRequest = new UpdateUserDtoRequest(
                "NewName", "NewLastName", "1234567890", "newemail@example.com");
            var updateResponse = new UpdatedDtoResponse(true, "Profile updated successfully.");
            var user = new User { Email = "newemail@example.com", Id = "1" };
            user.SetName("NewName");
            user.SetLastName("NewLastName");
            user.SetCpf("131.515.555-12");
            var tokenResponse = new TokenDtoResponse("accessToken", "refreshToken");

            _repositoryMock.Setup(r => r.UpdateProfileAsync(It.IsAny<UpdateUserDtoRequest>(), It.IsAny<string>()))
                .ReturnsAsync(updateResponse);
            _repositoryMock.Setup(r => r.GetUserProfileAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _tokenManagerServiceMock.Setup(tms => tms.GenerateTokenResponseAsync(It.IsAny<User>()))
                .ReturnsAsync(tokenResponse);

            // Act
            var result = await _authenticateService.UpdateUserServiceAsync(updateRequest, user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tokenResponse.Token, result.Token);
            Assert.Equal(tokenResponse.RefreshToken, result.RefreshToken);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowUnauthorizedAccessException_WhenUpdateFails()
        {
            // Arrange
            var updateRequest = new UpdateUserDtoRequest(
                "NewName", "NewLastName", "1234567890", "newemail@example.com");
            var updateResponse = new UpdatedDtoResponse(false, "Failed to update profile.");

            _repositoryMock.Setup(r => r.UpdateProfileAsync(It.IsAny<UpdateUserDtoRequest>(), It.IsAny<string>()))
                .ReturnsAsync(updateResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authenticateService.UpdateUserServiceAsync(updateRequest, "1")
            );
            Assert.Equal("Failed to update profile.", exception.Message);
        }
    }

    public class ChangePasswordAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnTrue_WhenPasswordChangeIsSuccessful()
        {
            // Arrange
            var user = new User { Email = "test@example.com", Id = "1" };
            var changePasswordRequest = new ChangePasswordDtoRequest("test@example.com", "oldPassword", "newPassword");

            _repositoryMock.Setup(r => r.GetUserIdProfileAsync(It.IsAny<string>())).ReturnsAsync(user);
            _repositoryMock.Setup(r => r.ChangePasswordAsync(It.IsAny<ChangePasswordDtoRequest>())).ReturnsAsync(true);

            // Act
            var result = await _authenticateService.ChangePasswordServiceAsync(changePasswordRequest, user.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldThrowUnauthorizedAccessException_WhenUserNotFound()
        {
            // Arrange
            var changePasswordRequest = new ChangePasswordDtoRequest("test@example.com", "oldPassword", "newPassword");

            _repositoryMock.Setup(r => r.GetUserIdProfileAsync(It.IsAny<string>())).ReturnsAsync((User)null!);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authenticateService.ChangePasswordServiceAsync(changePasswordRequest, "1")
            );
            Assert.Equal("User not found", exception.Message);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldThrowUnauthorizedAccessException_WhenEmailDoesNotMatch()
        {
            // Arrange
            var user = new User { Email = "different@example.com", Id = "1" };
            user.SetName("NewName");
            user.SetLastName("NewLastName");
            user.SetCpf("131.515.555-12");

            var changePasswordRequest = new ChangePasswordDtoRequest("test@example.com", "oldPassword", "newPassword");

            _repositoryMock.Setup(r => r.GetUserIdProfileAsync(It.IsAny<string>())).ReturnsAsync(user);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authenticateService.ChangePasswordServiceAsync(changePasswordRequest, user.Id)
            );
            Assert.Equal("Unauthorized attempt to change password", exception.Message);
        }
    }

    public class LogoutAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task LogoutAsync_ShouldCallRepositoryAndLog()
        {
            // Arrange
            _repositoryMock.Setup(r => r.LogoutAsync()).Returns(Task.CompletedTask);

            // Act
            await _authenticateService.LogoutServiceAsync();

            // Assert
            _repositoryMock.Verify(
                r => r.LogoutAsync(),
                Times.Once);
        }
    }

    public class ForgotPasswordAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task ForgotPasswordAsync_ShouldCallRepositoryAndLog()
        {
            // Arrange
            const string email = "test@example.com";
            const string newPassword = "newPassword";
            _repositoryMock.Setup(r => r.ForgotPasswordAsync(email, newPassword)).ReturnsAsync(true);

            // Act
            var result = await _authenticateService.ForgotPasswordServiceAsync(email, newPassword);

            // Assert
            Assert.True(result);

            _repositoryMock.Verify(
                r => r.ForgotPasswordAsync(email, newPassword),
                Times.Once);
        }
    }

    public class RefreshTokenAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowUnauthorizedAccessException_WhenRefreshTokenIsInvalid()
        {
            // Arrange
            var request = new RefreshTokenDtoRequest("validToken");
            _tokenServiceMock.Setup(ts => ts.ValidateToken(It.IsAny<string>())).Returns((ClaimsPrincipal)null!);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _authenticateService.RefreshTokenServiceAsync(request));
            Assert.Equal("[REFRESH_TOKEN] Error processing token refresh request.", exception.Message);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowUnauthorizedAccessException_WhenEmailClaimIsNotFound()
        {
            // Arrange
            var request = new RefreshTokenDtoRequest("validToken");
            var claims = new List<Claim>
            {
                Capacity = 0
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            _tokenServiceMock.Setup(ts => ts.ValidateToken(It.IsAny<string>())).Returns(claimsPrincipal);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _authenticateService.RefreshTokenServiceAsync(request));
            Assert.Equal("[REFRESH_TOKEN] Error processing token refresh request.", exception.Message);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotFound()
        {
            // Arrange
            var request = new RefreshTokenDtoRequest("validToken");
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.Email, "test@example.com")
            }));
            _tokenServiceMock.Setup(ts => ts.ValidateToken(It.IsAny<string>())).Returns(claimsPrincipal);
            _repositoryMock.Setup(r => r.GetUserProfileAsync(It.IsAny<string>())).ReturnsAsync((User)null!);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _authenticateService.RefreshTokenServiceAsync(request));
            Assert.Equal("[REFRESH_TOKEN] Error processing token refresh request.", exception.Message);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnTokenDtoResponse_WhenRefreshTokenIsValid()
        {
            // Arrange
            var request = new RefreshTokenDtoRequest("validToken");
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.Email, "test@example.com")
            }));
            var user = new User { Email = "test@example.com", Id = "1" };
            user.SetName("Test");
            user.SetLastName("User");
            var tokenResponse = new TokenDtoResponse("newAccessToken", "newRefreshToken");

            _tokenServiceMock.Setup(ts => ts.ValidateToken(It.IsAny<string>())).Returns(claimsPrincipal);
            _repositoryMock.Setup(r => r.GetUserProfileAsync(It.IsAny<string>())).ReturnsAsync(user);
            _tokenManagerServiceMock.Setup(tms => tms.GenerateTokenResponseAsync(It.IsAny<User>()))
                .ReturnsAsync(tokenResponse);

            // Act
            var result = await _authenticateService.RefreshTokenServiceAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newAccessToken", result.Token);
            Assert.Equal("newRefreshToken", result.RefreshToken);
        }
    }

    public class RevokedTokenAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task RevokedTokenAsync_ShouldReturnTrue_WhenTokenIsRevoked()
        {
            // Arrange
            const string tokenValue = "revokedToken";
            _tokenManagerServiceMock.Setup(tms => tms.RevokedTokenAsync(tokenValue)).ReturnsAsync(true);

            // Act
            var result = await _authenticateService.RevokedTokenServiceAsync(tokenValue);

            // Assert
            Assert.True(result);
            _tokenManagerServiceMock.Verify(tms => tms.RevokedTokenAsync(tokenValue), Times.Once);
        }

        [Fact]
        public async Task RevokedTokenAsync_ShouldReturnFalse_WhenTokenIsNotRevoked()
        {
            // Arrange
            const string tokenValue = "validToken";
            _tokenManagerServiceMock.Setup(tms => tms.RevokedTokenAsync(tokenValue)).ReturnsAsync(false);

            // Act
            var result = await _authenticateService.RevokedTokenServiceAsync(tokenValue);

            // Assert
            Assert.False(result);
            _tokenManagerServiceMock.Verify(tms => tms.RevokedTokenAsync(tokenValue), Times.Once);
        }
    }

    public class ExpiredTokenAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task ExpiredTokenAsync_ShouldReturnTrue_WhenTokenIsExpired()
        {
            // Arrange
            const string tokenValue = "expiredToken";
            _tokenManagerServiceMock.Setup(tms => tms.ExpiredTokenAsync(tokenValue)).ReturnsAsync(true);

            // Act
            var result = await _authenticateService.ExpiredTokenServiceAsync(tokenValue);

            // Assert
            Assert.True(result);
            _tokenManagerServiceMock.Verify(tms => tms.ExpiredTokenAsync(tokenValue), Times.Once);
        }

        [Fact]
        public async Task ExpiredTokenAsync_ShouldReturnFalse_WhenTokenIsNotExpired()
        {
            // Arrange
            const string tokenValue = "validToken";
            _tokenManagerServiceMock.Setup(tms => tms.ExpiredTokenAsync(tokenValue)).ReturnsAsync(false);

            // Act
            var result = await _authenticateService.ExpiredTokenServiceAsync(tokenValue);

            // Assert
            Assert.False(result);
            _tokenManagerServiceMock.Verify(tms => tms.ExpiredTokenAsync(tokenValue), Times.Once);
        }
    }
}