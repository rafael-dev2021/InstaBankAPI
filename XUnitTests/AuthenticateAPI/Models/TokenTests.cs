using AuthenticateAPI.Models;
using FluentAssertions;

namespace XUnitTests.AuthenticateAPI.Models;

public class TokenTests
{
    [Fact]
    public void Should_Set_And_Get_Id()
    {
        // Arrange
        var token = new Token();
        const int id = 123;

        // Act
        token.SetId(id);

        // Assert
        token.Id.Should().Be(id);
    }

    [Fact]
    public void Should_Set_And_Get_TokenValue()
    {
        // Arrange
        var token = new Token();
        const string tokenValue = "sample-token-value";

        // Act
        token.SetTokenValue(tokenValue);

        // Assert
        token.TokenValue.Should().Be(tokenValue);
    }

    [Fact]
    public void Should_Set_And_Get_TokenRevoked()
    {
        // Arrange
        var token = new Token();
        const bool tokenRevoked = true;

        // Act
        token.SetTokenRevoked(tokenRevoked);

        // Assert
        token.TokenRevoked.Should().Be(tokenRevoked);
    }

    [Fact]
    public void Should_Set_And_Get_TokenExpired()
    {
        // Arrange
        var token = new Token();
        const bool tokenExpired = true;

        // Act
        token.SetTokenExpired(tokenExpired);

        // Assert
        token.TokenExpired.Should().Be(tokenExpired);
    }

    [Fact]
    public void Should_Set_And_Get_TokenType()
    {
        // Arrange
        var token = new Token();
        const TokenType tokenType = TokenType.Bearer; 

        // Act
        token.SetTokenType(tokenType);

        // Assert
        token.TokenType.Should().Be(tokenType);
    }

    [Fact]
    public void Should_Set_And_Get_UserId()
    {
        // Arrange
        var token = new Token();
        const string userId = "user-123";

        // Act
        token.SetUserId(userId);

        // Assert
        token.UserId.Should().Be(userId);
    }

    [Fact]
    public void Should_Set_And_Get_User()
    {
        // Arrange
        var token = new Token();
        var user = new User();

        // Act
        token.SetUser(user);

        // Assert
        token.User.Should().Be(user);
    }
}

