using AuthenticateAPI.Context;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace XUnitTests.AuthenticateAPI.Repositories;

public class TokenRepositoryTests
{
    private static TokenRepository CreateTokenRepository(out AppDbContext dbContext)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        dbContext = new AppDbContext(options);
        return new TokenRepository(dbContext);
    }

    [Fact(DisplayName = "FindAllValidTokenByUser should return valid tokens for a user")]
    public async Task FindAllValidTokenByUser_Should_Return_Valid_Tokens()
    {
        // Arrange
        var tokenRepository = CreateTokenRepository(out var dbContext);
        const string userId = "1";

        var token1 = new Token();
        token1.SetUserId(userId);
        token1.SetTokenValue("Token1");
        token1.SetTokenExpired(false);
        token1.SetTokenRevoked(false);
        token1.SetTokenType(TokenType.Bearer);

        var token2 = new Token();
        token2.SetUserId(userId);
        token2.SetTokenValue("Token2");
        token2.SetTokenExpired(false);
        token2.SetTokenRevoked(false);
        token2.SetTokenType(TokenType.Bearer);

        dbContext.Tokens.AddRange(token1, token2);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await tokenRepository.FindAllValidTokenByUser(userId);

        // Assert
        result.Should().ContainSingle(t => t.TokenValue == "Token1");
    }

    [Fact(DisplayName = "FindAllTokensByUserId should return all tokens for a user")]
    public async Task FindAllTokensByUserId_Should_Return_All_Tokens()
    {
        // Arrange
        var tokenRepository = CreateTokenRepository(out var dbContext);
        const string userId = "1";

        var token1 = new Token();
        token1.SetId(1);
        token1.SetUserId(userId);
        token1.SetTokenValue("Token1");
        token1.SetTokenExpired(false);
        token1.SetTokenRevoked(false);
        token1.SetTokenType(TokenType.Bearer);

        var token2 = new Token();
        token2.SetId(2);
        token2.SetUserId(userId);
        token2.SetTokenValue("Token2");
        token2.SetTokenExpired(false);
        token2.SetTokenRevoked(false);
        token2.SetTokenType(TokenType.Bearer);

        dbContext.Tokens.AddRange(token1, token2);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await tokenRepository.FindAllTokensByUserId(userId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.TokenValue == "Token1" && t.UserId == userId);
        result.Should().Contain(t => t.TokenValue == "Token2" && t.UserId == userId);
    }

    [Fact(DisplayName = "FindByTokenValue should return token by its value")]
    public async Task FindByTokenValue_Should_Return_Token_By_Value()
    {
        // Arrange
        var tokenRepository = CreateTokenRepository(out var dbContext);
        const string tokenValue = "Token1";

        var token = new Token();
        token.SetTokenValue(tokenValue);
        token.SetTokenExpired(false);
        token.SetTokenRevoked(false);
        token.SetTokenType(TokenType.Bearer);

        dbContext.Tokens.Add(token);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await tokenRepository.FindByTokenValue(tokenValue);

        // Assert
        result.Should().NotBeNull();
        result!.TokenValue.Should().Be(tokenValue);
    }

    [Fact(DisplayName = "DeleteByUser should delete all tokens of a user")]
    public async Task DeleteByUser_Should_Delete_All_User_Tokens()
    {
        // Arrange
        var tokenRepository = CreateTokenRepository(out var dbContext);
        const string userId = "1";

        var token1 = new Token();
        token1.SetId(1);
        token1.SetUserId(userId);
        token1.SetTokenValue("Token1");
        token1.SetTokenExpired(false);
        token1.SetTokenRevoked(false);
        token1.SetTokenType(TokenType.Bearer);

        var token2 = new Token();
        token2.SetId(2);
        token2.SetUserId(userId);
        token2.SetTokenValue("Token2");
        token2.SetTokenExpired(false);
        token2.SetTokenRevoked(false);
        token2.SetTokenType(TokenType.Bearer);

        dbContext.Tokens.AddRange(token1, token2);
        await dbContext.SaveChangesAsync();

        var user = new User { Id = userId };

        // Act
        await tokenRepository.DeleteByUser(user);

        // Assert
        var remainingTokens = await dbContext.Tokens.ToListAsync();
        remainingTokens.Should().BeEmpty();
    }

    [Fact(DisplayName = "SaveAllTokensAsync should save all tokens")]
    public async Task SaveAllTokensAsync_Should_Save_All_Tokens()
    {
        // Arrange
        var tokenRepository = CreateTokenRepository(out var dbContext);
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

        // Act
        await tokenRepository.SaveAllTokensAsync(tokens);

        // Assert
        foreach (var token in tokens)
        {
            var savedToken = await dbContext.Tokens.FindAsync(token.Id);
            savedToken.Should().NotBeNull();
            savedToken!.TokenValue.Should().Be(token.TokenValue);
        }
    }
    
    [Fact(DisplayName = "SaveAllTokensAsync should update existing tokens")]
    public async Task SaveAllTokensAsync_Should_Update_Existing_Tokens()
    {
        // Arrange
        var tokenRepository = CreateTokenRepository(out var dbContext);
        var token1 = new Token();
        token1.SetId(1);
        token1.SetTokenValue("OldTokenValue1");
        token1.SetTokenExpired(false);
        token1.SetTokenRevoked(false);
        token1.SetTokenType(TokenType.Bearer);

        var token2 = new Token();
        token2.SetId(2);
        token2.SetTokenValue("OldTokenValue2");
        token2.SetTokenExpired(false);
        token2.SetTokenRevoked(false);
        token2.SetTokenType(TokenType.Bearer);

        // Add initial tokens to the context
        dbContext.Tokens.AddRange(token1, token2);
        await dbContext.SaveChangesAsync();

        // Modify tokens' values
        token1.SetTokenValue("UpdatedTokenValue1");
        token2.SetTokenValue("UpdatedTokenValue2");

        var updatedTokens = new List<Token>
        {
            token1, token2
        };

        // Act
        await tokenRepository.SaveAllTokensAsync(updatedTokens);

        // Assert
        foreach (var token in updatedTokens)
        {
            var savedToken = await dbContext.Tokens.FindAsync(token.Id);
            savedToken.Should().NotBeNull();
            savedToken!.TokenValue.Should().Be(token.TokenValue);
        }
    }

    [Fact(DisplayName = "DeleteAllTokensAsync should delete all tokens")]
    public async Task DeleteAllTokensAsync_Should_Delete_All_Tokens()
    {
        // Arrange
        var tokenRepository = CreateTokenRepository(out var dbContext);
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

        dbContext.Tokens.AddRange(tokens);
        await dbContext.SaveChangesAsync();

        // Act
        await tokenRepository.DeleteAllTokensAsync(tokens);

        // Assert
        var remainingTokens = await dbContext.Tokens.ToListAsync();
        remainingTokens.Should().BeEmpty();
    }

    [Fact(DisplayName = "SaveTokenAsync should save a token")]
    public async Task SaveTokenAsync_Should_Save_A_Token()
    {
        // Arrange
        var tokenRepository = CreateTokenRepository(out var dbContext);
        var token = new Token();
        token.SetId(1);
        token.SetTokenValue("Token1");
        token.SetTokenExpired(false);
        token.SetTokenRevoked(false);
        token.SetTokenType(TokenType.Bearer);

        // Act
        await tokenRepository.SaveTokenAsync(token);

        // Assert
        var savedToken = await dbContext.Tokens.FindAsync(token.Id);
        savedToken.Should().NotBeNull();
        savedToken!.TokenValue.Should().Be(token.TokenValue);
    }
    
    [Fact(DisplayName = "SaveTokenAsync should update existing token")]
    public async Task SaveTokenAsync_Should_Update_Existing_Token()
    {
        // Arrange
        var tokenRepository = CreateTokenRepository(out var dbContext);
        var token = new Token();
        token.SetId(1);
        token.SetTokenValue("OldTokenValue");
        token.SetTokenExpired(false);
        token.SetTokenRevoked(false);
        token.SetTokenType(TokenType.Bearer);

        dbContext.Tokens.Add(token);
        await dbContext.SaveChangesAsync();

        // Modify token's value
        var updatedToken = new Token();
        updatedToken.SetId(1);
        updatedToken.SetTokenValue("UpdatedTokenValue");
        updatedToken.SetTokenExpired(true); 
        updatedToken.SetTokenRevoked(true);
        updatedToken.SetTokenType(TokenType.Bearer);

        // Act
        await tokenRepository.SaveTokenAsync(updatedToken);

        // Assert
        var savedToken = await dbContext.Tokens.FindAsync(updatedToken.Id);
        savedToken.Should().NotBeNull();
        savedToken!.TokenValue.Should().Be("UpdatedTokenValue");
        savedToken.TokenExpired.Should().BeTrue();
        savedToken.TokenRevoked.Should().BeTrue();
    }
    
    [Fact(DisplayName = "SaveAsync should persist changes to the database")]
    public async Task SaveAsync_Should_Persist_Changes_To_Database()
    {
        // Arrange
        var tokenRepository = CreateTokenRepository(out var dbContext);
        var token = new Token();
        token.SetId(1);
        token.SetTokenValue("Token1");
        token.SetTokenExpired(false);
        token.SetTokenRevoked(false);
        token.SetTokenType(TokenType.Bearer);

        dbContext.Tokens.Add(token);
    
        // Act
        await tokenRepository.SaveAsync(); 

        // Assert
        var savedToken = await dbContext.Tokens.FindAsync(token.Id);
        savedToken.Should().NotBeNull();
        savedToken!.TokenValue.Should().Be("Token1");
    }
}
