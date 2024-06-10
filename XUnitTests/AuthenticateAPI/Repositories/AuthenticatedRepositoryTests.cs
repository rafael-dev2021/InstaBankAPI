using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories;
using FluentAssertions;
using Moq;

namespace XUnitTests.AuthenticateAPI.Repositories;

public class AuthenticatedRepositoryTests
{
    private readonly Mock<IAuthenticatedRepository> _authenticatedRepositoryMock = new();

    public class AuthenticateAsyncTests : AuthenticatedRepositoryTests
    {
        [Fact(DisplayName = "Should return authenticated result")]
        public async Task AuthenticateAsync_Should_Return_Authenticated_Result()
        {
            // Arrange
            var loginDtoRequest = new LoginDtoRequest("test@example.com", "password123", true);
            var expectedResult = new AuthenticatedDtoResponse(true, "");

            _authenticatedRepositoryMock.Setup(repo => repo.AuthenticateAsync(loginDtoRequest))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _authenticatedRepositoryMock.Object.AuthenticateAsync(loginDtoRequest);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }

    public class RegisterAsyncTests : AuthenticatedRepositoryTests
    {
        [Fact(DisplayName = "Should return registered result")]
        public async Task RegisterAsync_Should_Return_Registered_Result()
        {
            // Arrange
            var user = new User();
            user.SetName("John");
            user.SetLastName("Doe");
            user.SetEmail("john.doe@example.com");
            user.SetPhoneNumber("+1234567890");
            user.SetCpf("123.456.789-10");

            var expectedResult = new RegisteredDtoResponse(true, "");

            _authenticatedRepositoryMock.Setup(repo => repo.RegisterAsync(user))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _authenticatedRepositoryMock.Object.RegisterAsync(user);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }

    public class UpdateProfileAsyncTests : AuthenticatedRepositoryTests
    {
        [Fact(DisplayName = "Should return success for profile update")]
        public async Task UpdateProfileAsync_Should_Return_Success()
        {
            // Arrange
            var updateUserDtoRequest = new UpdateUserDtoRequest("John", "Doe", "john.doe@example.com", "+1234567890");
            var expectedResult = (success: true, errorMessage: "");

            _authenticatedRepositoryMock.Setup(repo => repo.UpdateProfileAsync(updateUserDtoRequest))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _authenticatedRepositoryMock.Object.UpdateProfileAsync(updateUserDtoRequest);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact(DisplayName = "Should return failure with error message for profile update")]
        public async Task UpdateProfileAsync_Should_Return_Failure_With_ErrorMessage()
        {
            // Arrange
            var updateUserDtoRequest = new UpdateUserDtoRequest("John", "Doe", "john.doe@example.com", "+1234567890");
            var expectedResult = (success: false, errorMessage: "Failed to update profile");

            _authenticatedRepositoryMock.Setup(repo => repo.UpdateProfileAsync(updateUserDtoRequest))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _authenticatedRepositoryMock.Object.UpdateProfileAsync(updateUserDtoRequest);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }

    public class ChangePasswordAsyncTests : AuthenticatedRepositoryTests
    {
        [Fact(DisplayName = "Should return true for password change")]
        public async Task ChangePasswordAsync_Should_Return_True()
        {
            // Arrange
            var changePasswordDtoRequest =
                new ChangePasswordDtoRequest("john.doe@example.com", "oldPassword", "newPassword");
            var expectedResult = true;

            _authenticatedRepositoryMock.Setup(repo => repo.ChangePasswordAsync(changePasswordDtoRequest))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _authenticatedRepositoryMock.Object.ChangePasswordAsync(changePasswordDtoRequest);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact(DisplayName = "Should return false for password change")]
        public async Task ChangePasswordAsync_Should_Return_False()
        {
            // Arrange
            var changePasswordDtoRequest =
                new ChangePasswordDtoRequest("john.doe@example.com", "oldPassword", "newPassword");
            var expectedResult = false;

            _authenticatedRepositoryMock.Setup(repo => repo.ChangePasswordAsync(changePasswordDtoRequest))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _authenticatedRepositoryMock.Object.ChangePasswordAsync(changePasswordDtoRequest);

            // Assert
            result.Should().Be(expectedResult);
        }
    }

    public class GetUserProfileAsyncTests : AuthenticatedRepositoryTests
    {
        [Fact(DisplayName = "Should return user profile")]
        public async Task GetUserProfileAsync_Should_Return_User_Profile()
        {
            // Arrange
            const string userEmail = "john.doe@example.com";
            var expectedUser = new User();
            expectedUser.SetName("John");
            expectedUser.SetLastName("Doe");
            expectedUser.SetEmail("john.doe@example.com");
            expectedUser.SetPhoneNumber("+1234567890");
            expectedUser.SetCpf("123.456.789-10");

            _authenticatedRepositoryMock.Setup(repo => repo.GetUserProfileAsync(userEmail))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _authenticatedRepositoryMock.Object.GetUserProfileAsync(userEmail);

            // Assert
            result.Should().BeEquivalentTo(expectedUser);
        }
    }

    public class ForgotPasswordAsyncTests : AuthenticatedRepositoryTests
    {
        [Fact(DisplayName = "Should return true for forgot password")]
        public async Task ForgotPasswordAsync_Should_Return_True()
        {
            // Arrange
            const string email = "john.doe@example.com";
            const string newPassword = "newPassword";
            const bool expectedSuccess = true;

            _authenticatedRepositoryMock.Setup(repo => repo.ForgotPasswordAsync(email, newPassword))
                .ReturnsAsync(expectedSuccess);

            // Act
            var result = await _authenticatedRepositoryMock.Object.ForgotPasswordAsync(email, newPassword);

            // Assert
            result.Should().Be(expectedSuccess);
        }

        [Fact(DisplayName = "Should return false for forgot password")]
        public async Task ForgotPasswordAsync_Should_Return_False()
        {
            // Arrange
            const string email = "john.doe@example.com";
            const string newPassword = "newPassword";
            const bool expectedSuccess = false;

            _authenticatedRepositoryMock.Setup(repo => repo.ForgotPasswordAsync(email, newPassword))
                .ReturnsAsync(expectedSuccess);

            // Act
            var result = await _authenticatedRepositoryMock.Object.ForgotPasswordAsync(email, newPassword);

            // Assert
            result.Should().Be(expectedSuccess);
        }
    }

    public class LogoutAsyncTests : AuthenticatedRepositoryTests
    {
        [Fact(DisplayName = "Should invoke logout method once")]
        public async Task LogoutAsync_Should_Invoke_Method_Once()
        {
            // Arrange
            _authenticatedRepositoryMock.Setup(repo => repo.LogoutAsync()).Verifiable();

            // Act
            await _authenticatedRepositoryMock.Object.LogoutAsync();

            // Assert
            _authenticatedRepositoryMock.Verify(repo => repo.LogoutAsync(), Times.Once);
        }
    }
}
