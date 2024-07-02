using System.Text;
using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Strategies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Serilog;

namespace XUnitTests.AuthenticateAPI.Repositories.Strategies;

public class AuthenticatedStrategyTests
{
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly AuthenticatedStrategy _authenticatedStrategy;

    public AuthenticatedStrategyTests()
    {
        var store = new Mock<IUserStore<User>>();
        Mock<UserManager<User>> userManagerMock = new(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var contextAccessor = new Mock<IHttpContextAccessor>();
        var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
        _signInManagerMock = new Mock<SignInManager<User>>(
            userManagerMock.Object,
            contextAccessor.Object,
            userPrincipalFactory.Object,
            null!, null!, null!, null!);

        _cacheMock = new Mock<IDistributedCache>();

        _authenticatedStrategy = new AuthenticatedStrategy(_signInManagerMock.Object, _cacheMock.Object);

        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
    }

    [Fact]
    public async Task AuthenticatedAsync_SuccessfulLogin_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new LoginDtoRequest("user@example.com", "password", true);
        _signInManagerMock.Setup(s => s.PasswordSignInAsync(request.Email!, request.Password!, request.RememberMe, false))
            .ReturnsAsync(SignInResult.Success);

        _cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), default)).ReturnsAsync((byte[])null!);

        // Act
        var result = await _authenticatedStrategy.AuthenticatedAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Login successful.", result.Message);
        _cacheMock.Verify(c => c.RemoveAsync(It.IsAny<string>(), default), Times.Once);
    }

    [Fact]
    public async Task AuthenticatedAsync_InvalidPassword_ReturnsFailureResponse()
    {
        // Arrange
        var request = new LoginDtoRequest("user@example.com", "wrongpassword", true);
        
        _signInManagerMock.Setup(s => s.PasswordSignInAsync(request.Email!, request.Password!, request.RememberMe, false))
            .ReturnsAsync(SignInResult.Failed);
        
        _cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), default)).ReturnsAsync((byte[])null!);

        // Act
        var result = await _authenticatedStrategy.AuthenticatedAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid email or password. Please try again.", result.Message);
        _cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default), Times.Once);
    }

    [Fact]
    public async Task AuthenticatedAsync_AccountLocked_ReturnsLockedResponse()
    {
        // Arrange
        var request = new LoginDtoRequest("user@example.com", "wrongpassword", true);
        
        _cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), default)).ReturnsAsync("3"u8.ToArray());

        // Act
        var result = await _authenticatedStrategy.AuthenticatedAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Your account is locked. Please contact support.", result.Message);
    }

    [Fact]
    public async Task IncrementLoginAttemptsAsync_CacheIncrementsAttempts()
    {
        // Arrange
        const string cacheKey = "login_attempts_user@example.com";
        
        _cacheMock.Setup(c => c.GetAsync(cacheKey, default)).ReturnsAsync("2"u8.ToArray());

        // Act
        await _authenticatedStrategy.IncrementLoginAttemptsAsync(cacheKey);

        // Assert
        _cacheMock.Verify(c => c.SetAsync(cacheKey, Encoding.UTF8.GetBytes("3"), It.IsAny<DistributedCacheEntryOptions>(), default), Times.Once);
    }
    
    [Fact]
    public async Task IncrementLoginAttemptsAsync_ExceptionThrown_LogsErrorAndThrows()
    {
        // Arrange
        const string cacheKey = "login_attempts_user@example.com";
        var exception = new Exception("Cache increment error");

        _cacheMock.Setup(c => c.GetAsync(cacheKey, default)).ThrowsAsync(exception);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _authenticatedStrategy.IncrementLoginAttemptsAsync(cacheKey));
        Assert.Equal(exception.Message, ex.Message);
        _cacheMock.Verify(c => c.GetAsync(cacheKey, default), Times.Once);
    }
    
    [Fact]
    public async Task GetLoginAttemptsAsync_ReturnsCorrectLoginAttempts()
    {
        // Arrange
        const string cacheKey = "login_attempts_user@example.com";
        const int expectedAttempts = 3;
        _cacheMock.Setup(c => c.GetAsync(cacheKey, default))
            .ReturnsAsync(Encoding.UTF8.GetBytes(expectedAttempts.ToString()));

        // Act
        var result = await _authenticatedStrategy.GetLoginAttemptsAsync(cacheKey);

        // Assert
        Assert.Equal(expectedAttempts, result);
        _cacheMock.Verify(c => c.GetAsync(cacheKey, default), Times.Once);
    }

    [Fact]
    public async Task GetLoginAttemptsAsync_ExceptionThrown_LogsErrorAndThrows()
    {
        // Arrange
        const string cacheKey = "login_attempts_user@example.com";
        var exception = new Exception("Cache retrieval error");

        _cacheMock.Setup(c => c.GetAsync(cacheKey, default)).ThrowsAsync(exception);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _authenticatedStrategy.GetLoginAttemptsAsync(cacheKey));
        Assert.Equal(exception.Message, ex.Message);
        _cacheMock.Verify(c => c.GetAsync(cacheKey, default), Times.Once);
    }

    [Fact]
    public async Task ResetLoginAttemptsAsync_CacheResetsAttempts()
    {
        // Arrange
        const string cacheKey = "login_attempts_user@example.com";

        // Act
        await _authenticatedStrategy.ResetLoginAttemptsAsync(cacheKey);

        // Assert
        _cacheMock.Verify(c => c.RemoveAsync(cacheKey, default), Times.Once);
    }
    
    [Fact]
    public async Task ResetLoginAttemptsAsync_ExceptionThrown_LogsErrorAndThrows()
    {
        // Arrange
        const string cacheKey = "login_attempts_user@example.com";
        var exception = new Exception("Cache remove error");

        _cacheMock.Setup(c => c.RemoveAsync(cacheKey, default)).ThrowsAsync(exception);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _authenticatedStrategy.ResetLoginAttemptsAsync(cacheKey));
        Assert.Equal(exception.Message, ex.Message);
        _cacheMock.Verify(c => c.RemoveAsync(cacheKey, default), Times.Once);
    }
}
