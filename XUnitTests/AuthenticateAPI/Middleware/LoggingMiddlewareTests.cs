using System.Text;
using AuthenticateAPI.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace XUnitTests.AuthenticateAPI.Middleware;

public class LoggingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldLogRequestDuration()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var loggerMock = new Mock<ILogger<LoggingMiddleware>>();
        var next = new RequestDelegate(_ => Task.CompletedTask);
        var middleware = new LoggingMiddleware(next, loggerMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        loggerMock.Verify(
            x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!),
            Times.AtLeastOnce);
        loggerMock.Verify(
            x => x.Log(LogLevel.Information, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Request Duration")), null,
                ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!),
            Times.Once);
    }

    [Fact]
    public async Task LogAuditEvent_ShouldLogRequestDetails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var loggerMock = new Mock<ILogger>();
        var request = context.Request;
        request.Method = "GET";
        request.Path = "/api/test";
        request.QueryString = new QueryString("?param1=value1");
        request.Headers.Append("User-Agent", "TestUserAgent");
        var response = context.Response;
        response.StatusCode = 200;

        const string expectedLogMessage1 = $"Audit Event: GET /api/test?param1=value1 - UserAgent: TestUserAgent";
        const string expectedLogMessage2 = "Request Body: {\"key\":\"value\"}";

        // Simulate request body
        const string requestBody = "{\"key\":\"value\"}";
        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));
        request.Body = memoryStream;
        request.ContentLength = memoryStream.Length;

        var logger = loggerMock.Object;

        // Act
        await LoggingMiddleware.LogAuditEvent(request, response, logger);

        // Assert
        loggerMock.Verify(
            x => x.Log(LogLevel.Information, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(expectedLogMessage1)), null,
                ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!), Times.Once);
        loggerMock.Verify(
            x => x.Log(LogLevel.Information, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(expectedLogMessage2)), null,
                ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!), Times.Once);
    }

    [Fact]
    public async Task GetRequestBodyAsync_ShouldReturnCorrectBody()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var request = context.Request;
        const string expectedBody = "{\"key\":\"value\"}";
        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(expectedBody));
        request.Body = memoryStream;
        request.ContentLength = memoryStream.Length;

        // Act
        var requestBody = await LoggingMiddleware.GetRequestBodyAsync(request);

        // Assert
        Assert.Equal(expectedBody, requestBody);
    }
}