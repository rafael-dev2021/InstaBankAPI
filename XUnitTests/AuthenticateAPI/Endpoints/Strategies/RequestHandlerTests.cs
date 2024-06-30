using System.Security.Claims;
using AuthenticateAPI.Endpoints.Strategies;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace XUnitTests.AuthenticateAPI.Endpoints.Strategies;

public class RequestHandlerTests
{
    private class TestRequest
    {
        public string Name { get; init; } = string.Empty;
    }

    private class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    [Fact]
    public async Task HandleRequestAsync_InvalidRequest_ReturnsValidationProblem()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "123") }, "TestAuthType");
        context.User = new ClaimsPrincipal(identity);
        var request = new TestRequest { Name = string.Empty };
        var validator = new TestRequestValidator();

        // Act
        var (result, userId) = await RequestHandler.HandleRequestAsync(context, request, validator);

        // Assert
        Assert.NotNull(result);
        var validationProblem = result as IStatusCodeHttpResult;
        Assert.NotNull(validationProblem);
        Assert.Equal(StatusCodes.Status400BadRequest, validationProblem.StatusCode);
        Assert.Null(userId);
    }

    [Fact]
    public async Task HandleRequestAsync_UserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity())
        };
        var request = new TestRequest { Name = "Valid Name" };
        var validator = new TestRequestValidator();

        // Act
        var (result, userId) = await RequestHandler.HandleRequestAsync(context, request, validator);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, (result as StatusCodeHttpResult)?.StatusCode);
        Assert.Null(userId);
    }

    [Fact]
    public async Task HandleRequestAsync_UserIdNotFound_ReturnsUnauthorized()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "User") }, "TestAuthType");
        context.User = new ClaimsPrincipal(identity);
        var request = new TestRequest { Name = "Valid Name" };
        var validator = new TestRequestValidator();

        // Act
        var (result, userId) = await RequestHandler.HandleRequestAsync(context, request, validator);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, (result as StatusCodeHttpResult)?.StatusCode);
        Assert.Null(userId);
    }

    [Fact]
    public async Task HandleRequestAsync_ValidRequest_ReturnsNullAndUserId()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "123") }, "TestAuthType");
        context.User = new ClaimsPrincipal(identity);
        var request = new TestRequest { Name = "Valid Name" };
        var validator = new TestRequestValidator();

        // Act
        var (result, userId) = await RequestHandler.HandleRequestAsync(context, request, validator);

        // Assert
        Assert.Null(result);
        Assert.Equal("123", userId);
    }

    [Fact]
    public async Task HandleServiceCallAsync_ServiceCallSucceeds_ReturnsResult()
    {
        // Arrange
        var expectedResult = Results.Ok();

        // Act
        var result = await RequestHandler.HandleServiceCallAsync(ServiceCall);

        // Assert
        Assert.Equal(expectedResult, result);
        return;

        Task<IResult> ServiceCall() => Task.FromResult(expectedResult);
    }

    [Fact]
    public async Task HandleServiceCallAsync_UnauthorizedAccessException_ReturnsForbidden()
    {
        // Arrange
        Func<Task<IResult>> serviceCall = () => throw new UnauthorizedAccessException("Access denied");

        // Act
        var result = await RequestHandler.HandleServiceCallAsync(serviceCall);

        // Assert
        Assert.NotNull(result);
        var jsonResult = result as JsonHttpResult<Dictionary<string, string>>;
        Assert.NotNull(jsonResult);
        Assert.Equal(StatusCodes.Status403Forbidden, jsonResult.StatusCode);
        var responseDict = jsonResult.Value;
        Assert.Equal("Access denied", responseDict?["Message"]);
    }
}