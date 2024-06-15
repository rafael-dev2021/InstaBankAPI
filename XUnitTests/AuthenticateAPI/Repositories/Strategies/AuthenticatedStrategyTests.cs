using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Strategies;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace XUnitTests.AuthenticateAPI.Repositories.Strategies;

public class AuthenticatedStrategyTests
{
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly AuthenticatedStrategy _authenticatedStrategy;

    public AuthenticatedStrategyTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!,
            null!, null!, null!);
        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();

        _signInManagerMock = new Mock<SignInManager<User>>(
            userManagerMock.Object,
            contextAccessorMock.Object,
            claimsFactoryMock.Object,
            null!,
            null!,
            null!,
            null!
        );

        _authenticatedStrategy = new AuthenticatedStrategy(_signInManagerMock.Object);
    }
    
    [Fact(DisplayName = "Should return success response when login is successful")]
    public async Task AuthenticatedAsync_Should_Return_Success_Response()
    {
        // Arrange
        var request = new LoginDtoRequest("john@example.com", "password123", true);
        _signInManagerMock
            .Setup(s => s.PasswordSignInAsync(request.Email!, request.Password!, request.RememberMe, true))
            .ReturnsAsync(SignInResult.Success);

        // Act
        var response = await _authenticatedStrategy.AuthenticatedAsync(request);

        // Assert
        response.Should().BeEquivalentTo(new AuthenticatedDtoResponse(true, "Login successful."));
    }

    [Fact(DisplayName = "Should return error response when login fails")]
    public async Task AuthenticatedAsync_Should_Return_Error_Response_When_Login_Fails()
    {
        // Arrange
        var request = new LoginDtoRequest("john@example.com", "wrongpassword", true);
        _signInManagerMock
            .Setup(s => s.PasswordSignInAsync(request.Email!, request.Password!, request.RememberMe, true))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var response = await _authenticatedStrategy.AuthenticatedAsync(request);

        // Assert
        response.Should().BeEquivalentTo(new AuthenticatedDtoResponse(false, "Invalid email or password. Please try again."));
    }

    [Fact(DisplayName = "Should return error response when account is locked")]
    public async Task AuthenticatedAsync_Should_Return_Error_Response_When_Account_Is_Locked()
    {
        // Arrange
        var request = new LoginDtoRequest("john@example.com", "password123", true);
        _signInManagerMock
            .Setup(s => s.PasswordSignInAsync(request.Email!, request.Password!, request.RememberMe, true))
            .ReturnsAsync(SignInResult.LockedOut);

        // Act
        var response = await _authenticatedStrategy.AuthenticatedAsync(request);

        // Assert
        response.Should().BeEquivalentTo(new AuthenticatedDtoResponse(false, "Your account is locked. Please contact support."));
    }
}