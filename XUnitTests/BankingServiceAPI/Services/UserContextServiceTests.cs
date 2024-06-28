using System.Security.Claims;
using BankingServiceAPI.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace XUnitTests.BankingServiceAPI.Services;

public class UserContextServiceTests
{
    private readonly Mock<HttpContext> _httpContextMock = new();
    private readonly UserContextService _userContextService = new();

    [Fact]
    public async Task GetUserFromHttpContextAsync_Should_Return_User_From_HttpContext()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "1"),
            new(ClaimTypes.GivenName, "John"),
            new(ClaimTypes.Surname, "Doe"),
            new("Cpf", "123.456.789-00"),
            new("PhoneNumber", "123456789"),
            new(ClaimTypes.Email, "john.doe@example.com"),
            new(ClaimTypes.Role, "Admin")
        };

        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _httpContextMock.Setup(x => x.User).Returns(claimsPrincipal);

        // Act
        var user = await _userContextService.GetUserFromHttpContextAsync(_httpContextMock.Object);

        // Assert
        Assert.NotNull(user);
        Assert.Equal("1", user.Id);
        Assert.Equal("John", user.Name);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("123.456.789-00", user.Cpf);
        Assert.Equal("123456789", user.PhoneNumber);
        Assert.Equal("john.doe@example.com", user.Email);
        Assert.Equal("Admin", user.Role);
    }
}