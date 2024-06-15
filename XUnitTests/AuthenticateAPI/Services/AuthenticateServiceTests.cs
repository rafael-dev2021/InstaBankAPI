using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using AuthenticateAPI.Security;
using AuthenticateAPI.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace XUnitTests.AuthenticateAPI.Services
{
    public class AuthenticateServiceTests
    {
        private readonly Mock<IAuthenticatedRepository> _repositoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly AuthenticateService _authenticateService;
        private readonly Mock<ILogger<AuthenticateService>> _loggerMock;

        protected AuthenticateServiceTests()
        {
            _repositoryMock = new Mock<IAuthenticatedRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _loggerMock = new Mock<ILogger<AuthenticateService>>();
            _authenticateService = new AuthenticateService(_repositoryMock.Object, _tokenServiceMock.Object, _loggerMock.Object);
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

                // Verify logging
                _loggerMock.Verify(
                    x => x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((o, t) => o.ToString() == $"LoginAsync called with email: {loginRequest.Email}"),
                        It.IsAny<Exception>(),
                        ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!),
                    Times.Once);

                _loggerMock.Verify(
                    x => x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((o, t) => o.ToString() == $"Login successful for email: {loginRequest.Email}"),
                        It.IsAny<Exception>(),
                        ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!),
                    Times.Once);
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

                // Verify logging
                _loggerMock.Verify(
                    x => x.Log(
                        LogLevel.Warning,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((o, t) => o.ToString() == $"Authentication failed for email: {loginRequest.Email} with message: {authResponse.Message}"),
                        It.IsAny<Exception>(),
                        ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!),
                    Times.Once);
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

                // Verify logging
                _loggerMock.Verify(
                    x => x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((o, t) => o.ToString() == $"RegisterAsync called with email: {registerRequest.Email}"),
                        It.IsAny<Exception>(),
                        ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!),
                    Times.Once);

                _loggerMock.Verify(
                    x => x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((o, t) => o.ToString() == $"Registration successful for email: {registerRequest.Email}"),
                        It.IsAny<Exception>(),
                        ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!),
                    Times.Once);
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

                // Verify logging
                _loggerMock.Verify(
                    x => x.Log(
                        LogLevel.Warning,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((o, t) => o.ToString() == $"Registration failed for email: {registerRequest.Email} with message: {registerResponse.Message}"),
                        It.IsAny<Exception>(),
                        ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!),
                    Times.Once);
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

                // Verify logging
                _loggerMock.Verify(
                    x => x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((o, t) => o.ToString() == $"UpdateAsync called for userId: {user.Id}"),
                        It.IsAny<Exception>(),
                        ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!),
                    Times.Once);

                _loggerMock.Verify(
                    x => x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((o, t) => o.ToString() == $"Profile update successful for userId: {user.Id}"),
                        It.IsAny<Exception>(),
                        ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!),
                    Times.Once);
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

                // Verify logging
                _loggerMock.Verify(
                    x => x.Log(
                        LogLevel.Warning,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((o, t) => o.ToString() == $"Profile update failed for userId: 1 with message: {updateResponse.Message}"),
                        It.IsAny<Exception>(),
                        ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!),
                    Times.Once);
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

                // Verify logging
                _loggerMock.Verify(
                    x => x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((o, t) => o.ToString() == $"ChangePasswordAsync called for email: {changePasswordRequest.Email}"),
                        It.IsAny<Exception>(),
                        ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!),
                    Times.Once);

                _loggerMock.Verify(
                    x => x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((o, t) => o.ToString() == $"Password change successful for email: {changePasswordRequest.Email}"),
                        It.IsAny<Exception>(),
                        ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!),
                    Times.Once);
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

                // Verify logging
                _loggerMock.Verify(
                    x => x.Log(
                        LogLevel.Warning,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((o, t) => o.ToString() == $"Password change failed for email: {changePasswordRequest.Email}"),
                        It.IsAny<Exception>(),
                        ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!),
                    Times.Once);
            }
        }
    }
}
