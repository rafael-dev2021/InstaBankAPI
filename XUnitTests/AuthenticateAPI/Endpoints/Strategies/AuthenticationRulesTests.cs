using System.Security.Claims;
using AuthenticateAPI.Endpoints.Strategies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace XUnitTests.AuthenticateAPI.Endpoints.Strategies;

public class AuthenticationRulesTests
{
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

    [Fact]
    public void CheckAdminRole_ReturnsNull_WhenUserIsAdmin()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Admin") };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
        context.User = principal;

        // Act
        var result = AuthenticationRules.CheckAdminRole(context);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void CheckAdminRole_ReturnsForbidden_WhenUserIsNotAdmin()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var claims = new List<Claim> { new Claim(ClaimTypes.Role, "User") };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
        context.User = principal;

        // Act
        var result = AuthenticationRules.CheckAdminRole(context);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<JsonHttpResult<Dictionary<string, string>>>(result);
        var jsonResult = result as JsonHttpResult<Dictionary<string, string>>;
        Assert.Equal(StatusCodes.Status403Forbidden, jsonResult?.StatusCode);
    }
}