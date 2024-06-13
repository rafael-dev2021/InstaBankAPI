using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories;
using AuthenticateAPI.Repositories.Strategies;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace XUnitTests.AuthenticateAPI.Repositories.Integration;

public class AuthenticatedRepositoryTests
{
    private readonly Mock<IAuthenticatedStrategy> _authenticatedStrategyMock;
    private readonly AuthenticatedRepository _authenticatedRepository;
    private readonly Mock<IRegisterStrategy> _registerStrategyMock;
    private readonly Mock<IUpdateProfileStrategy> _updateProfileStrategyMock;


    protected AuthenticatedRepositoryTests()
    {
        _registerStrategyMock = new Mock<IRegisterStrategy>();
        _authenticatedStrategyMock = new Mock<IAuthenticatedStrategy>();
        _updateProfileStrategyMock = new Mock<IUpdateProfileStrategy>();


        var userStoreMock = new Mock<IUserStore<User>>();
        var userManagerMock =
            new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
        var signInManagerMock = new Mock<SignInManager<User>>(
            userManagerMock.Object,
            contextAccessorMock.Object,
            userPrincipalFactoryMock.Object,
            null!, null!, null!, null!
        );

        _authenticatedRepository = new AuthenticatedRepository(
            signInManagerMock.Object,
            userManagerMock.Object,
            _authenticatedStrategyMock.Object,
            _registerStrategyMock.Object,
            _updateProfileStrategyMock.Object
        );
    }

    public class AuthenticateAsyncTests : AuthenticatedRepositoryTests
    {
        [Fact(DisplayName = "AuthenticateAsync should return success response when login is successful")]
        public async Task AuthenticateAsync_Should_Return_Success_Response()
        {
            // Arrange
            var request = new LoginDtoRequest("john@example.com", "password123", true);
            var expectedResponse = new AuthenticatedDtoResponse(true, "Login successful.");

            _authenticatedStrategyMock
                .Setup(s => s.AuthenticatedAsync(request))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _authenticatedRepository.AuthenticateAsync(request);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
            _authenticatedStrategyMock.Verify(s => s.AuthenticatedAsync(request), Times.Once);
        }

        [Fact(DisplayName = "AuthenticateAsync should return error response when login fails")]
        public async Task AuthenticateAsync_Should_Return_Error_Response()
        {
            // Arrange
            var request = new LoginDtoRequest("john@example.com", "wrongpassword", true);
            var expectedResponse = new AuthenticatedDtoResponse(false, "Invalid email or password. Please try again.");

            _authenticatedStrategyMock
                .Setup(s => s.AuthenticatedAsync(request))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _authenticatedRepository.AuthenticateAsync(request);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
            _authenticatedStrategyMock.Verify(s => s.AuthenticatedAsync(request), Times.Once);
        }
    }

    public class RegisterAsyncTests : AuthenticatedRepositoryTests
    {
        [Fact(DisplayName = "RegisterAsync should return success response when registration is successful")]
        public async Task RegisterAsync_Should_Return_Success_Response()
        {
            // Arrange
            var request = new RegisterDtoRequest("John", "Doe", "+1234567890", "123.456.789-01", "john.doe@example.com",
                "StrongP@ssw0rd", "StrongP@ssw0rd");

            var expectedResponse = new RegisteredDtoResponse(true, "Registration successful.");

            _registerStrategyMock
                .Setup(s => s.ValidateAsync(request.Cpf, request.Email, request.PhoneNumber))
                .ReturnsAsync([]);

            _registerStrategyMock
                .Setup(s => s.CreateUserAsync(request))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _authenticatedRepository.RegisterAsync(request);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
            _registerStrategyMock.Verify(s => s.ValidateAsync(request.Cpf, request.Email, request.PhoneNumber),
                Times.Once);
            _registerStrategyMock.Verify(s => s.CreateUserAsync(request), Times.Once);
        }
        
        [Fact(DisplayName = "RegisterAsync should return error response when validation fails")]
        public async Task RegisterAsync_Should_Return_Error_Response()
        {
            // Arrange
            var request = new RegisterDtoRequest("John", "Doe", "+1234567890", "123.456.789-01", "john.doe@example.com",
                "StrongP@ssw0rd", "StrongP@ssw0r");

            var validationErrors = new List<string> { "Email already used." };
            var expectedResponse = new RegisteredDtoResponse(false, string.Join(Environment.NewLine, validationErrors));

            _registerStrategyMock
                .Setup(s => s.ValidateAsync(request.Cpf, request.Email, request.PhoneNumber))
                .ReturnsAsync(validationErrors);

            // Act
            var response = await _authenticatedRepository.RegisterAsync(request);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
            _registerStrategyMock.Verify(s => s.ValidateAsync(request.Cpf, request.Email, request.PhoneNumber), Times.Once);
            _registerStrategyMock.Verify(s => s.CreateUserAsync(It.IsAny<RegisterDtoRequest>()), Times.Never);
        }
    }
    
     public class UpdateProfileAsyncTests : AuthenticatedRepositoryTests
    {
        [Fact(DisplayName = "UpdateProfileAsync should return success response when profile update is successful")]
        public async Task UpdateProfileAsync_Should_Return_Success_Response()
        {
            // Arrange
            const string userId = "12345";
            var request = new UpdateUserDtoRequest("NewName", "NewLastName", "new.email@example.com", "+1234567890");
            var expectedResponse = new UpdatedDtoResponse(true, "Profile updated successfully.");

            _updateProfileStrategyMock
                .Setup(s => s.UpdateProfileAsync(request, userId))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _authenticatedRepository.UpdateProfileAsync(request, userId);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
            _updateProfileStrategyMock.Verify(s => s.UpdateProfileAsync(request, userId), Times.Once);
        }

        [Fact(DisplayName = "UpdateProfileAsync should return error response when profile update fails")]
        public async Task UpdateProfileAsync_Should_Return_Error_Response()
        {
            // Arrange
            const string userId = "12345";
            var request = new UpdateUserDtoRequest("NewName", "NewLastName", "new.email@example.com", "+1234567890");
            var validationErrors = new List<string> { "Email already used by another user." };
            var expectedResponse = new UpdatedDtoResponse(false, string.Join(Environment.NewLine, validationErrors));

            _updateProfileStrategyMock
                .Setup(s => s.UpdateProfileAsync(request, userId))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _authenticatedRepository.UpdateProfileAsync(request, userId);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
            _updateProfileStrategyMock.Verify(s => s.UpdateProfileAsync(request, userId), Times.Once);
        }
    }
}