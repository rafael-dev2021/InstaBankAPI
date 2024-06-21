using System.Net;
using System.Security.Claims;
using AuthenticateAPI.Middleware;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using AuthenticateAPI.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace XUnitTests.AuthenticateAPI.Middleware;

public class SecurityFilterMiddlewareTests
{
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<ITokenRepository> _mockTokenRepository;
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<ILogger<SecurityFilterMiddleware>> _mockLogger;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<HttpRequest> _mockHttpRequest;
    private readonly Mock<HttpResponse> _mockHttpResponse;
    private readonly Mock<RequestDelegate> _mockNext;
    private readonly SecurityFilterMiddleware _middleware;

    protected SecurityFilterMiddlewareTests()
    {
        _mockTokenService = new Mock<ITokenService>();
        _mockTokenRepository = new Mock<ITokenRepository>();
        _mockUserManager = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null!, null!, null!, null!, null!, null!, null!, null!
        );
        _mockLogger = new Mock<ILogger<SecurityFilterMiddleware>>();
        _mockHttpContext = new Mock<HttpContext>();
        _mockHttpRequest = new Mock<HttpRequest>();
        _mockHttpResponse = new Mock<HttpResponse>();
        _mockNext = new Mock<RequestDelegate>();
        _middleware = new SecurityFilterMiddleware(_mockNext.Object, _mockLogger.Object);

        _mockHttpContext.Setup(c => c.Request).Returns(_mockHttpRequest.Object);
        _mockHttpContext.Setup(c => c.Response).Returns(_mockHttpResponse.Object);

        var mockConnection = new Mock<ConnectionInfo>();
        mockConnection.Setup(c => c.RemoteIpAddress).Returns(IPAddress.Parse("127.0.0.1"));
        _mockHttpContext.Setup(c => c.Connection).Returns(mockConnection.Object);

        var headers = new HeaderDictionary
        {
            { "User-Agent", "MockUserAgent" }
        };
        _mockHttpRequest.Setup(r => r.Headers).Returns(headers);

        var responseBody = new MemoryStream();
        _mockHttpResponse.Setup(r => r.Body).Returns(responseBody);
    }

    public class InvokeAsyncTests : SecurityFilterMiddlewareTests
    {
        [Fact]
        public async Task Should_Handle_Authentication_For_Valid_Token()
        {
            // Arrange
            const string validToken = "validToken";
            const string email = "user@example.com";
            var user = new User { UserName = "user", Email = email };

            var token = new Token();
            token.SetTokenValue(validToken);
            token.SetTokenExpired(false);
            token.SetTokenRevoked(false);

            var validPrincipal =
                new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) }, "Bearer"));

            _mockHttpRequest.Setup(r => r.Headers.Authorization).Returns($"Bearer {validToken}");
            _mockTokenService.Setup(ts => ts.ValidateToken(validToken)).Returns(validPrincipal);
            _mockUserManager.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(user);
            _mockTokenRepository.Setup(tr => tr.FindByTokenValue(validToken)).ReturnsAsync(token);

            var filter = new SecurityFilterMiddleware(_mockNext.Object, _mockLogger.Object);

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(ITokenService))).Returns(_mockTokenService.Object);
            mockServiceProvider.Setup(sp => sp.GetService(typeof(ITokenRepository)))
                .Returns(_mockTokenRepository.Object);
            mockServiceProvider.Setup(sp => sp.GetService(typeof(UserManager<User>))).Returns(_mockUserManager.Object);

            _mockHttpContext.Setup(c => c.RequestServices).Returns(mockServiceProvider.Object);

            // Act
            await filter.InvokeAsync(_mockHttpContext.Object, mockServiceProvider.Object);

            // Assert
            _mockNext.Verify(next => next(_mockHttpContext.Object), Times.Once);
        }

        [Fact]
        public async Task Should_Log_Error_On_Invocation_Failure()
        {
            // Arrange
            const string exceptionMessage = "Test exception";
            var exception = new Exception(exceptionMessage);

            var filter = new SecurityFilterMiddleware(_mockNext.Object, _mockLogger.Object);

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(ITokenService)))
                .Throws(exception);

            _mockHttpContext.Setup(c => c.RequestServices).Returns(mockServiceProvider.Object);

            // Act
            await filter.InvokeAsync(_mockHttpContext.Object, mockServiceProvider.Object);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(exceptionMessage)),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
                Times.Once);
        }
        
        [Fact]
        public async Task InvokeAsync_TokenIsNotNullAndAuthenticationFails_CallsNext()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var serviceProvider = new ServiceCollection()
                .AddSingleton(_mockTokenService.Object)
                .AddSingleton(_mockTokenRepository.Object)
                .AddSingleton(_mockUserManager.Object)
                .BuildServiceProvider();

            context.RequestServices = serviceProvider;
            context.Request.Headers.Authorization = "Bearer invalid-token";

            _mockTokenService.Setup(service => service.ValidateToken(It.IsAny<string>()))
                .Throws(new SecurityTokenException("Invalid token"));

            // Act
            await _middleware.InvokeAsync(context, serviceProvider);

            // Assert
            _mockNext.Verify(next => next(context), Times.Once);
        }
    }

    public class RecoverTokenFromRequestTests : SecurityFilterMiddlewareTests
    {
        [Fact]
        public void Should_Return_Token_When_Authorization_Header_Present()
        {
            // Arrange
            var request = new Mock<HttpRequest>();
            var headers = new HeaderDictionary
            {
                { "Authorization", "Bearer validToken" }
            };
            request.Setup(r => r.Headers).Returns(headers);

            var securityFilter = new SecurityFilterMiddleware(_mockNext.Object, _mockLogger.Object);

            // Act
            var token = securityFilter.RecoverTokenFromRequest(request.Object);

            // Assert
            Assert.NotNull(token);
            Assert.Equal("validToken", token);

            _mockLogger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Debug),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, t) =>
                        state.ToString()!.Contains(
                            "[RECOVERED] Token recovered from Authorization header: validToken")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!),
                Times.Once);
        }
    }

    public class HandleAuthenticationTests : SecurityFilterMiddlewareTests
    {
        [Fact]
        public async Task HandleAuthentication_UserNotFound_ShouldLogErrorAndReturnFalse()
        {
            // Arrange
            const string token = "invalidToken";
            const string email = "user@example.com";

            _mockTokenService.Setup(ts => ts.ValidateToken(token))
                .Returns(new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) })));
            _mockUserManager.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync((User)null!);

            var filter = new SecurityFilterMiddleware(_mockNext.Object, _mockLogger.Object);

            // Act
            var result = await filter.HandleAuthentication(_mockHttpContext.Object, token, _mockTokenService.Object,
                _mockTokenRepository.Object, _mockUserManager.Object);

            // Assert
            Assert.False(result);
            _mockHttpResponse.VerifySet(r => r.StatusCode = StatusCodes.Status403Forbidden, Times.Once);
            _mockLogger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, t) => state.ToString()!.Contains("[TOKEN_FAILED]")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!),
                Times.Once);
        }

        [Fact]
        public async Task HandleAuthentication_ValidTokenAndUserFound_ShouldReturnTrue()
        {
            // Arrange
            const string token = "validToken";
            const string email = "user@example.com";
            var user = new User { UserName = "user", Email = email };

            var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) });
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var validToken = new Token();
            validToken.SetTokenValue(token);
            validToken.SetTokenExpired(false);
            validToken.SetTokenRevoked(false);

            _mockTokenService.Setup(ts => ts.ValidateToken(token)).Returns(claimsPrincipal);
            _mockUserManager.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(user);
            _mockTokenRepository.Setup(tr => tr.FindByTokenValue(token)).ReturnsAsync(validToken);

            var filter = new SecurityFilterMiddleware(_mockNext.Object, _mockLogger.Object);

            // Act
            var result = await filter.HandleAuthentication(_mockHttpContext.Object, token, _mockTokenService.Object,
                _mockTokenRepository.Object, _mockUserManager.Object);

            // Assert
            Assert.True(result);
            _mockLogger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, t) =>
                        state.ToString()!.Contains(
                            "[USER_AUTHENTICATED] User: user successfully authenticated with token: validToken.")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!),
                Times.Once);
        }

        [Fact]
        public async Task HandleAuthentication_Should_Return_False_For_Invalid_Token_Format()
        {
            // Arrange
            var context = new DefaultHttpContext();
            const string token = "invalid_format_token";

            _mockTokenService.Setup(s => s.ValidateToken(token))
                .Throws<SecurityTokenException>();

            // Act
            var result = await _middleware.HandleAuthentication(context, token, _mockTokenService.Object,
                _mockTokenRepository.Object, _mockUserManager.Object);

            // Assert
            Assert.False(result);
            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        }
    }
}