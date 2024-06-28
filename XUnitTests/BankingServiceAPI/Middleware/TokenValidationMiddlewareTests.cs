using System.Net;
using BankingServiceAPI.Middleware;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Protected;
using Xunit.Abstractions;

namespace XUnitTests.BankingServiceAPI.Middleware;

public class TokenValidationMiddlewareTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly TokenValidationMiddleware _middleware;

    public TokenValidationMiddlewareTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _nextMock = new Mock<RequestDelegate>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        
        Environment.SetEnvironmentVariable("BASE_URL", "https://localhost:7074");

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri(Environment.GetEnvironmentVariable("BASE_URL")!)
        };
        
        _middleware = new TokenValidationMiddleware(_nextMock.Object, httpClient);
    }

    [Fact]
    public async Task InvokeAsync_WithValidToken_ShouldCallNext()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Authorization = "Bearer validToken";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Headers.Authorization != null &&
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri("https://localhost:7074/v1/auth/revoked-token") &&
                    req.Headers.Authorization.Parameter == "validToken"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Headers.Authorization != null &&
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri("https://localhost:7074/v1/auth/expired-token") &&
                    req.Headers.Authorization.Parameter == "validToken"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        await _middleware.InvokeAsync(context);

        _nextMock.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithRevokedToken_ShouldReturnUnauthorized()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Authorization = "Bearer revokedToken";
        context.Response.Body = new MemoryStream(); 

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Headers.Authorization != null &&
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri("https://localhost:7074/v1/auth/revoked-token") &&
                    req.Headers.Authorization.Parameter == "revokedToken"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized));

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Headers.Authorization != null &&
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri("https://localhost:7074/v1/auth/expired-token") &&
                    req.Headers.Authorization.Parameter == "revokedToken"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        await _middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin); 
        var response = await new StreamReader(context.Response.Body).ReadToEndAsync();
    
        _testOutputHelper.WriteLine("Response: " + response);

        Assert.Equal("Token is expired or revoked", response);
        _nextMock.Verify(next => next(context), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_WithExpiredToken_ShouldReturnUnauthorized()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Authorization = "Bearer expiredToken";
        context.Response.Body = new MemoryStream(); 

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Headers.Authorization != null &&
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri("https://localhost:7074/v1/auth/revoked-token") &&
                    req.Headers.Authorization.Parameter == "expiredToken"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Headers.Authorization != null &&
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri("https://localhost:7074/v1/auth/expired-token") &&
                    req.Headers.Authorization.Parameter == "expiredToken"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized));

        await _middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin); 
        var response = await new StreamReader(context.Response.Body).ReadToEndAsync();
    
        _testOutputHelper.WriteLine("Response: " + response);

        Assert.Equal("Token is expired or revoked", response);
        _nextMock.Verify(next => next(context), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_WithoutAuthorizationHeader_ShouldCallNext()
    {
        var context = new DefaultHttpContext();

        await _middleware.InvokeAsync(context);

        _nextMock.Verify(next => next(context), Times.Once);
    }
}