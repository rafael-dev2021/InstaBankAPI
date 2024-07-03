using System.Security.Claims;
using BankingServiceAPI.Endpoints.Strategies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace XUnitTests.BankingServiceAPI.Endpoints.Strategies;

public class AuthenticationRulesTests
{
    [Fact]
    public void CheckAdminRole_UserIsAdmin_ReturnsNull()
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

        // Act
        var result = AuthenticationRules.CheckAdminRole(context);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void CheckAdminRole_UserIsNotAdmin_ReturnsForbidden()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "User") }, "TestAuthType");
        var user = new ClaimsPrincipal(identity);
        context.User = user;

        // Act
        var result = AuthenticationRules.CheckAdminRole(context);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<JsonHttpResult<Dictionary<string, string>>>(result);
        var jsonResult = result as JsonHttpResult<Dictionary<string, string>>;
        Assert.Equal(StatusCodes.Status403Forbidden, jsonResult?.StatusCode);
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
