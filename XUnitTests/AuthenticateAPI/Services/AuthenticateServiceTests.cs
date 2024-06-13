using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories;
using AuthenticateAPI.Services;
using Moq;

namespace XUnitTests.AuthenticateAPI.Services;

public class AuthenticateServiceTests
{
    private readonly Mock<IAuthenticatedRepository> _repositoryMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly AuthenticateService _authenticateService;

    protected AuthenticateServiceTests()
    {
        _repositoryMock = new Mock<IAuthenticatedRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _authenticateService = new AuthenticateService(_repositoryMock.Object, _tokenServiceMock.Object);
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
            const string expectedAccessToken = "accessToken";
            const string expectedRefreshToken = "refreshToken";

            _repositoryMock.Setup(r => r.AuthenticateAsync(It.IsAny<LoginDtoRequest>()))
                .ReturnsAsync(authResponse);
            _repositoryMock.Setup(r => r.GetUserProfileAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _tokenServiceMock.Setup(ts => ts.GenerateAccessToken(It.IsAny<User>()))
                .Returns(expectedAccessToken);
            _tokenServiceMock.Setup(ts => ts.GenerateRefreshToken(It.IsAny<User>()))
                .Returns(expectedRefreshToken);

            // Act
            var result = await _authenticateService.LoginAsync(loginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedAccessToken, result.Token);
            Assert.Equal(expectedRefreshToken, result.RefreshToken);
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
                () => _authenticateService.LoginAsync(loginRequest)
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
                "Test", "Test", "1234567890", "131.515.555-12", "test@example.com", "password", "password");
            var registerResponse = new RegisteredDtoResponse(true, "Registration successful.");
            var user = new User { Email = "test@example.com", Id = "1" };
            user.SetName("Test");
            user.SetLastName("Test");
            user.SetCpf("131.515.555-12");
            const string expectedAccessToken = "accessToken";
            const string expectedRefreshToken = "refreshToken";

            _repositoryMock.Setup(r => r.RegisterAsync(It.IsAny<RegisterDtoRequest>()))
                .ReturnsAsync(registerResponse);
            _repositoryMock.Setup(r => r.GetUserProfileAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _tokenServiceMock.Setup(ts => ts.GenerateAccessToken(It.IsAny<User>()))
                .Returns(expectedAccessToken);
            _tokenServiceMock.Setup(ts => ts.GenerateRefreshToken(It.IsAny<User>()))
                .Returns(expectedRefreshToken);

            // Act
            var result = await _authenticateService.RegisterAsync(registerRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedAccessToken, result.Token);
            Assert.Equal(expectedRefreshToken, result.RefreshToken);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowUnauthorizedAccessException_WhenRegistrationFails()
        {
            // Arrange
            var registerRequest = new RegisterDtoRequest(
                "Test", "Test", "1234567890", "131.515.555-12", "test@example.com", "password", "password");
            var registerResponse = new RegisteredDtoResponse(false, "Registration failed.");

            _repositoryMock.Setup(r => r.RegisterAsync(It.IsAny<RegisterDtoRequest>()))
                .ReturnsAsync(registerResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authenticateService.RegisterAsync(registerRequest)
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
            const string expectedAccessToken = "accessToken";
            const string expectedRefreshToken = "refreshToken";

            _repositoryMock.Setup(r => r.UpdateProfileAsync(It.IsAny<UpdateUserDtoRequest>(), It.IsAny<string>()))
                .ReturnsAsync(updateResponse);
            _repositoryMock.Setup(r => r.GetUserProfileAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _tokenServiceMock.Setup(ts => ts.GenerateAccessToken(It.IsAny<User>()))
                .Returns(expectedAccessToken);
            _tokenServiceMock.Setup(ts => ts.GenerateRefreshToken(It.IsAny<User>()))
                .Returns(expectedRefreshToken);

            // Act
            var result = await _authenticateService.UpdateAsync(updateRequest, user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedAccessToken, result.Token);
            Assert.Equal(expectedRefreshToken, result.RefreshToken);
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
                () => _authenticateService.UpdateAsync(updateRequest, "1")
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
            var changePasswordRequest = new ChangePasswordDtoRequest("test@example.com", "oldPassword", "newPassword");

            _repositoryMock.Setup(r => r.ChangePasswordAsync(It.IsAny<ChangePasswordDtoRequest>()))
                .ReturnsAsync(true);

            // Act
            var result = await _authenticateService.ChangePasswordAsync(changePasswordRequest);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnFalse_WhenPasswordChangeFails()
        {
            // Arrange
            var changePasswordRequest = new ChangePasswordDtoRequest("test@example.com", "oldPassword", "newPassword");

            _repositoryMock.Setup(r => r.ChangePasswordAsync(It.IsAny<ChangePasswordDtoRequest>()))
                .ReturnsAsync(false);

            // Act
            var result = await _authenticateService.ChangePasswordAsync(changePasswordRequest);

            // Assert
            Assert.False(result);
        }
    }
}