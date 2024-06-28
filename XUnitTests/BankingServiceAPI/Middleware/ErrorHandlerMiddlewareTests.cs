using System.Net;
using System.Text.Json;
using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace XUnitTests.BankingServiceAPI.Middleware;

public class ErrorHandlerMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly ErrorHandlerMiddleware _middleware;

    public ErrorHandlerMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        Mock<ILogger<ErrorHandlerMiddleware>> loggerMock = new();
        _middleware = new ErrorHandlerMiddleware(_nextMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task Invoke_WithNoException_ShouldCallNext()
    {
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        await _middleware.Invoke(context);

        _nextMock.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task Invoke_WithBalanceInsufficientException_ShouldReturnBadRequest()
    {
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .Throws(new BalanceInsufficientException("Insufficient balance"));

        await _middleware.Invoke(context);

        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var response = await new StreamReader(context.Response.Body).ReadToEndAsync();

        Assert.False(string.IsNullOrWhiteSpace(response), "The response body is empty or whitespace.");

        var responseModel = JsonSerializer.Deserialize<JsonElement>(response);
        Assert.Equal("Insufficient balance", responseModel.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Invoke_WithAccountNotFoundException_ShouldReturnNotFound()
    {
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .Throws(new AccountNotFoundException("Account not found"));

        await _middleware.Invoke(context);

        Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var response = await new StreamReader(context.Response.Body).ReadToEndAsync();

        Assert.False(string.IsNullOrWhiteSpace(response), "The response body is empty or whitespace.");

        var responseModel = JsonSerializer.Deserialize<JsonElement>(response);
        Assert.Equal("Account not found", responseModel.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Invoke_WithUnexpectedException_ShouldReturnInternalServerError()
    {
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        _nextMock.Setup(next => next(It.IsAny<HttpContext>())).Throws(new Exception("Unexpected error"));

        await _middleware.Invoke(context);

        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var response = await new StreamReader(context.Response.Body).ReadToEndAsync();

        Assert.False(string.IsNullOrWhiteSpace(response), "The response body is empty or whitespace.");

        var responseModel = JsonSerializer.Deserialize<JsonElement>(response);
        Assert.Equal("An error occurred. Please try again later.", responseModel.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Invoke_WithBankAccountDtoServiceException_ShouldReturnBadRequest()
    {
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .Throws(new BankAccountDtoServiceException("Invalid account details", null!));

        await _middleware.Invoke(context);

        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var response = await new StreamReader(context.Response.Body).ReadToEndAsync();

        Assert.False(string.IsNullOrWhiteSpace(response), "The response body is empty or whitespace.");

        var responseModel = JsonSerializer.Deserialize<JsonElement>(response);
        Assert.Equal("Invalid account details", responseModel.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Invoke_WithGetIdNotFoundException_ShouldReturnNotFound()
    {
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        _nextMock.Setup(next => next(It.IsAny<HttpContext>())).Throws(new GetIdNotFoundException("ID not found"));

        await _middleware.Invoke(context);

        Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var response = await new StreamReader(context.Response.Body).ReadToEndAsync();

        Assert.False(string.IsNullOrWhiteSpace(response), "The response body is empty or whitespace.");

        var responseModel = JsonSerializer.Deserialize<JsonElement>(response);
        Assert.Equal("ID not found", responseModel.GetProperty("message").GetString());
    }
}