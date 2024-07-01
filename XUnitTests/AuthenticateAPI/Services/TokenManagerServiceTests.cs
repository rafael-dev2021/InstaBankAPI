using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using AuthenticateAPI.Security;
using AuthenticateAPI.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace XUnitTests.AuthenticateAPI.Services;

public class TokenManagerServiceTests
{
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<ITokenRepository> _tokenRepositoryMock;
    private readonly Mock<IAuthenticatedRepository> _authenticatedRepositoryMock;
    private readonly TokenManagerService _tokenManagerService;

    public TokenManagerServiceTests()
    {
        _tokenServiceMock = new Mock<ITokenService>();
        _tokenRepositoryMock = new Mock<ITokenRepository>();
        _authenticatedRepositoryMock = new Mock<IAuthenticatedRepository>();
        Mock<ILogger<TokenManagerService>> loggerMock = new();

        _tokenManagerService = new TokenManagerService(
            _tokenServiceMock.Object,
            _tokenRepositoryMock.Object,
            _authenticatedRepositoryMock.Object,
            loggerMock.Object
        );
    }

    [Fact]
    public async Task GenerateTokenResponseAsync_ShouldGenerateAndSaveTokens()
    {
        // Arrange
        var user = new User { Id = "1", Email = "user@example.com" };
        user.SetName("Test");
        user.SetLastName("Test");

        const string accessToken = "accessToken";
        const string refreshToken = "refreshToken";

        _tokenServiceMock.Setup(ts => ts.GenerateAccessToken(user)).Returns(accessToken);
        _tokenServiceMock.Setup(ts => ts.GenerateRefreshToken(user)).Returns(refreshToken);

        _tokenRepositoryMock.Setup(tr => tr.FindAllTokensByUserId(user.Id)).ReturnsAsync(new List<Token>());
        _tokenRepositoryMock.Setup(tr => tr.SaveTokenAsync(It.IsAny<Token>())).ReturnsAsync((Token t) => t);
        _authenticatedRepositoryMock.Setup(ar => ar.SaveAsync(user)).Returns(Task.CompletedTask);

        // Act
        var result = await _tokenManagerService.GenerateTokenResponseAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accessToken, result.Token);
        Assert.Equal(refreshToken, result.RefreshToken);

        _tokenServiceMock.Verify(ts => ts.GenerateAccessToken(user), Times.Once);
        _tokenServiceMock.Verify(ts => ts.GenerateRefreshToken(user), Times.Once);
        _authenticatedRepositoryMock.Verify(ar => ar.SaveAsync(user), Times.Once);
    }

    [Fact]
    public void RevokeAllUserTokens_ShouldRevokeAllTokens()
    {
        // Arrange
        var user = new User { Id = "1" };

        var token1 = new Token();
        token1.SetId(1);
        token1.SetTokenValue("Token1");
        token1.SetTokenExpired(false);
        token1.SetTokenRevoked(false);
        token1.SetTokenType(TokenType.Bearer);

        var token2 = new Token();
        token2.SetId(2);
        token2.SetTokenValue("Token2");
        token2.SetTokenExpired(false);
        token2.SetTokenRevoked(false);
        token2.SetTokenType(TokenType.Bearer);

        var tokens = new List<Token> { token1, token2 };

        _tokenRepositoryMock.Setup(tr => tr.FindAllValidTokenByUser(user.Id)).ReturnsAsync(tokens);
        _tokenRepositoryMock.Setup(tr => tr.SaveAllTokensAsync(It.IsAny<List<Token>>())).Returns(Task.CompletedTask);

        // Act
        _tokenManagerService.RevokeAllUserTokens(user);

        // Assert
        foreach (var token in tokens)
        {
            Assert.True(token.TokenExpired);
            Assert.True(token.TokenRevoked);
        }

        _tokenRepositoryMock.Verify(tr => tr.FindAllValidTokenByUser(user.Id), Times.Once);
        _tokenRepositoryMock.Verify(tr => tr.SaveAllTokensAsync(It.IsAny<List<Token>>()), Times.Once);
    }

    [Fact]
    public void RevokeAllUserTokens_ShouldHandleNoTokensGracefully()
    {
        // Arrange
        var user = new User { Id = "1" };

        var tokens = new List<Token>();

        _tokenRepositoryMock.Setup(tr => tr.FindAllValidTokenByUser(user.Id)).ReturnsAsync(tokens);

        // Act
        _tokenManagerService.RevokeAllUserTokens(user);

        // Assert
        _tokenRepositoryMock.Verify(tr => tr.FindAllValidTokenByUser(user.Id), Times.Once);
        _tokenRepositoryMock.Verify(tr => tr.SaveAllTokensAsync(It.IsAny<List<Token>>()), Times.Never);
    }

    [Fact]
    public async Task ClearTokensAsync_ShouldDeleteAllTokens()
    {
        // Arrange
        const string userId = "1";
        var token1 = new Token();
        token1.SetId(1);
        token1.SetTokenValue("Token1");
        token1.SetTokenExpired(false);
        token1.SetTokenRevoked(false);
        token1.SetTokenType(TokenType.Bearer);

        var token2 = new Token();
        token2.SetId(2);
        token2.SetTokenValue("Token2");
        token2.SetTokenExpired(false);
        token2.SetTokenRevoked(false);
        token2.SetTokenType(TokenType.Bearer);

        var tokens = new List<Token>
        {
            token1, token2
        };

        _tokenRepositoryMock.Setup(tr => tr.FindAllTokensByUserId(userId)).ReturnsAsync(tokens);
        _tokenRepositoryMock.Setup(tr => tr.DeleteAllTokensAsync(It.IsAny<List<Token>>())).Returns(Task.CompletedTask);

        // Act
        await _tokenManagerService.ClearTokensAsync(userId);

        // Assert
        _tokenRepositoryMock.Verify(tr => tr.FindAllTokensByUserId(userId), Times.Once);
        _tokenRepositoryMock.Verify(tr => tr.DeleteAllTokensAsync(tokens), Times.Once);
    }

    [Fact]
    public async Task SaveUserTokenAsync_ShouldSaveToken()
    {
        // Arrange
        var user = new User { Id = "1" };
        const string jwtToken = "jwtToken";
        var token = new Token();
        token.SetTokenValue(jwtToken);
        token.SetUser(user);
        token.SetUserId(user.Id);

        _tokenRepositoryMock.Setup(tr => tr.SaveTokenAsync(It.IsAny<Token>())).ReturnsAsync((Token t) =>
        {
            t.SetUserId(user.Id);
            return t;
        });

        // Act
        var result = await _tokenManagerService.SaveUserTokenAsync(user, jwtToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(jwtToken, result.TokenValue);
        Assert.Equal(user.Id, result.UserId);

        _tokenRepositoryMock.Verify(tr => tr.SaveTokenAsync(It.IsAny<Token>()), Times.Once);
    }

    [Fact]
    public async Task RevokedTokenAsync_ShouldReturnTrue_WhenTokenIsRevoked()
    {
        // Arrange
        const string tokenValue = "revokedToken";
        var revokedToken = new Token();
        revokedToken.SetTokenValue(tokenValue);
        revokedToken.SetTokenRevoked(true);

        _tokenRepositoryMock.Setup(tr => tr.FindByTokenValue(tokenValue)).ReturnsAsync(revokedToken);

        // Act
        var result = await _tokenManagerService.RevokedTokenAsync(tokenValue);

        // Assert
        Assert.True(result);
        _tokenRepositoryMock.Verify(tr => tr.FindByTokenValue(tokenValue), Times.Once);
    }

    [Fact]
    public async Task RevokedTokenAsync_ShouldReturnFalse_WhenTokenIsNotRevoked()
    {
        // Arrange
        const string tokenValue = "validToken";
        var validToken = new Token();
        validToken.SetTokenValue(tokenValue);
        validToken.SetTokenRevoked(false);

        _tokenRepositoryMock.Setup(tr => tr.FindByTokenValue(tokenValue)).ReturnsAsync(validToken);

        // Act
        var result = await _tokenManagerService.RevokedTokenAsync(tokenValue);

        // Assert
        Assert.False(result);
        _tokenRepositoryMock.Verify(tr => tr.FindByTokenValue(tokenValue), Times.Once);
    }

    [Fact]
    public async Task ExpiredTokenAsync_ShouldReturnTrue_WhenTokenIsExpired()
    {
        // Arrange
        const string tokenValue = "expiredToken";
        var expiredToken = new Token();
        expiredToken.SetTokenValue(tokenValue);
        expiredToken.SetTokenExpired(true);

        _tokenRepositoryMock.Setup(tr => tr.FindByTokenValue(tokenValue)).ReturnsAsync(expiredToken);

        // Act
        var result = await _tokenManagerService.ExpiredTokenAsync(tokenValue);

        // Assert
        Assert.True(result);
        _tokenRepositoryMock.Verify(tr => tr.FindByTokenValue(tokenValue), Times.Once);
    }

    [Fact]
    public async Task ExpiredTokenAsync_ShouldReturnFalse_WhenTokenIsNotExpired()
    {
        // Arrange
        const string tokenValue = "validToken";
        var validToken = new Token();
        validToken.SetTokenValue(tokenValue);
        validToken.SetTokenExpired(false);

        _tokenRepositoryMock.Setup(tr => tr.FindByTokenValue(tokenValue)).ReturnsAsync(validToken);

        // Act
        var result = await _tokenManagerService.ExpiredTokenAsync(tokenValue);

        // Assert
        Assert.False(result);
        _tokenRepositoryMock.Verify(tr => tr.FindByTokenValue(tokenValue), Times.Once);
    }
}