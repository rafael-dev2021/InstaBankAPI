using AuthenticateAPI.Middleware;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace XUnitTests.AuthenticateAPI.Middleware;

public class LogoutHandlerMiddlewareTests
{
    private readonly Mock<ILogger<LogoutHandlerMiddleware>> _loggerMock = new();
    private readonly Mock<ITokenRepository> _tokenRepositoryMock = new();
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly Mock<RequestDelegate> _nextMock = new();

    public LogoutHandlerMiddlewareTests()
    {
        Mock<IUserStore<User>> userStoreMock = new();
        UserManager<User> userManager = new(
            userStoreMock.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!
        );

        _signInManagerMock = new Mock<SignInManager<User>>(
            userManager,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<User>>(),
            null!,
            null!,
            null!,
            null!
        );
    }

    [Fact]
    public async Task InvokeAsync_LogoutPath_ValidToken_InvalidatesTokenAndLogs()
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            Request =
            {
                Path = "/v1/auth/logout"
            }
        };
        context.Request.Headers.Append("Authorization", "Bearer validToken");

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ITokenRepository)))
            .Returns(_tokenRepositoryMock.Object);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(SignInManager<User>)))
            .Returns(_signInManagerMock.Object);

        var validToken = new Token();
        _tokenRepositoryMock.Setup(repo => repo.FindByTokenValue("validToken"))
            .ReturnsAsync(validToken);

        var handler = new LogoutHandlerMiddleware(_nextMock.Object, _loggerMock.Object);

        // Act
        await handler.InvokeAsync(context, serviceProviderMock.Object);

        // Assert
        _tokenRepositoryMock.Verify(repo => repo.FindByTokenValue("validToken"), Times.Once);
        _tokenRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);
        _signInManagerMock.Verify(manager => manager.SignOutAsync(), Times.Once);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_LogoutPath_InvalidToken_SendsUnauthorizedResponse()
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            Request =
            {
                Path = "/v1/auth/logout",
                Headers =
                {
                    Authorization = "Bearer invalidToken"
                }
            }
        };

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ITokenRepository)))
            .Returns(_tokenRepositoryMock.Object);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(SignInManager<User>)))
            .Returns(_signInManagerMock.Object);

        _tokenRepositoryMock.Setup(repo => repo.FindByTokenValue("invalidToken"))
            .ReturnsAsync((Token)null!);

        var handler = new LogoutHandlerMiddleware(_nextMock.Object, _loggerMock.Object);

        // Act
        await handler.InvokeAsync(context, serviceProviderMock.Object);

        // Assert
        _tokenRepositoryMock.Verify(repo => repo.FindByTokenValue("invalidToken"), Times.Once);
        _tokenRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Never);
        _signInManagerMock.Verify(manager => manager.SignOutAsync(), Times.Never);
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_NonLogoutPath_CallsNextDelegate()
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            Request =
            {
                Path = "/some/other/path"
            }
        };

        var handler = new LogoutHandlerMiddleware(_nextMock.Object, _loggerMock.Object);

        // Act
        await handler.InvokeAsync(context, Mock.Of<IServiceProvider>());

        // Assert
        _nextMock.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_LogoutPath_NoAuthHeader_SendsUnauthorizedResponse()
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            Request =
            {
                Path = "/v1/auth/logout"
            }
        };

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ITokenRepository)))
            .Returns(_tokenRepositoryMock.Object);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(SignInManager<User>)))
            .Returns(_signInManagerMock.Object);

        var handler = new LogoutHandlerMiddleware(_nextMock.Object, _loggerMock.Object);

        // Act
        await handler.InvokeAsync(context, serviceProviderMock.Object);

        // Assert
        _tokenRepositoryMock.Verify(repo => repo.FindByTokenValue(It.IsAny<string>()), Times.Never);
        _tokenRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Never);
        _signInManagerMock.Verify(manager => manager.SignOutAsync(), Times.Never);
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        _loggerMock.Verify(logger => logger.Log(
            LogLevel.Debug,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) =>
                v.ToString()!.Contains("[NO_AUTH_HEADER] No Authorization header found in the request.")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()!
        ));
    }
}