using System.Security.Claims;
using BankingServiceAPI.Endpoints.Strategies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace XUnitTests.BankingServiceAPI.Endpoints.Strategies;

public class AuthenticationRulesTests
{
    [Fact]
    public void CheckAuthenticationAndAuthorization_NoAuthenticationRequired_ReturnsNull()
    {
        // Arrange
        var context = new DefaultHttpContext();
        const bool requireAuthentication = false;

        // Act
        var result = AuthenticationRules.CheckAuthenticationAndAuthorization(context, requireAuthentication);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void CheckAuthenticationAndAuthorization_NotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity())
        };
        const bool requireAuthentication = true;

        // Act
        var result = AuthenticationRules.CheckAuthenticationAndAuthorization(context, requireAuthentication);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<JsonHttpResult<Dictionary<string, string>>>(result);
        var jsonResult = result as JsonHttpResult<Dictionary<string, string>>;
        Assert.Equal(StatusCodes.Status401Unauthorized, jsonResult?.StatusCode);
    }

    [Fact]
    public void CheckAuthenticationAndAuthorization_AuthenticatedNotAdmin_ReturnsForbidden()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "User") }, "TestAuthType");
        var user = new ClaimsPrincipal(identity);
        context.User = user;
        const bool requireAuthentication = true;

        // Act
        var result = AuthenticationRules.CheckAuthenticationAndAuthorization(context, requireAuthentication);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<JsonHttpResult<Dictionary<string, string>>>(result);
        var jsonResult = result as JsonHttpResult<Dictionary<string, string>>;
        Assert.Equal(StatusCodes.Status403Forbidden, jsonResult?.StatusCode);
    }

    [Fact]
    public void CheckAuthenticationAndAuthorization_AuthenticatedAdmin_ReturnsNull()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "Admin"),
            new Claim(ClaimTypes.Role, "Admin")
        }, "TestAuthType");
        var user = new ClaimsPrincipal(identity);
        context.User = user;
        const bool requireAuthentication = true;

        // Act
        var result = AuthenticationRules.CheckAuthenticationAndAuthorization(context, requireAuthentication);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void HandleUnauthorizedAccessException_ReturnsForbiddenWithMessage()
    {
        // Arrange
        const string exceptionMessage = "Unauthorized access.";
        var exception = new UnauthorizedAccessException(exceptionMessage);

        // Act
        var result = AuthenticationRules.HandleUnauthorizedAccessException(exception);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<JsonHttpResult<Dictionary<string, string>>>(result);
        var jsonResult = result as JsonHttpResult<Dictionary<string, string>>;
        Assert.Equal(StatusCodes.Status403Forbidden, jsonResult?.StatusCode);
    }
}