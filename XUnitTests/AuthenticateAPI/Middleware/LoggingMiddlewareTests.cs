using System.Text;
using AuthenticateAPI.Middleware;
using Microsoft.AspNetCore.Http;
using Moq;

namespace XUnitTests.AuthenticateAPI.Middleware;

public class LoggingMiddlewareTests
{
    
    [Fact]
    public async Task InvokeAsync_ShouldLogRequestDuration()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var request = context.Request;
        request.Method = "GET";
        request.Path = "/api/test";
        request.QueryString = new QueryString("?param1=value1");
        request.Headers.Append("User-Agent", "TestUserAgent");
        var response = context.Response;
        response.StatusCode = 200;

        var memoryStream = new MemoryStream("{\"key\":\"value\"}"u8.ToArray());
        request.Body = memoryStream;
        request.ContentLength = memoryStream.Length;

        var next = new Mock<RequestDelegate>();
        next.Setup(x => x.Invoke(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        var loggingMiddleware = new LoggingMiddleware(next.Object);

        // Act
        await loggingMiddleware.InvokeAsync(context);

        // Assert
        next.Verify(x => x.Invoke(It.IsAny<HttpContext>()), Times.Once);
    }
    
    [Fact]
    public async Task LogAuditEvent_ShouldLogRequestDetails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var request = context.Request;
        request.Method = "GET";
        request.Path = "/api/test";
        request.QueryString = new QueryString("?param1=value1");
        request.Headers.Append("User-Agent", "TestUserAgent");
        var response = context.Response;
        response.StatusCode = 200;

        // Simulate request body
        const string requestBody = "{\"key\":\"value\"}";
        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));
        request.Body = memoryStream;
        request.ContentLength = memoryStream.Length;

        // Act
        await LoggingMiddleware.LogAuditEvent(request, response);
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