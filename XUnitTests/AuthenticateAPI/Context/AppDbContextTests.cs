using AuthenticateAPI.Context;
using AuthenticateAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace XUnitTests.AuthenticateAPI.Context;

public class AppDbContextTests
{
    private static DbContextOptions<AppDbContext> GetInMemoryDbContextOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    }
    
    [Fact]
    public void CanCreateDatabase()
    {
        // Arrange
        var options = GetInMemoryDbContextOptions();

        // Act
        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();
        }

        // Assert
        using (var context = new AppDbContext(options))
        {
            Assert.True(context.Database.CanConnect());
        }
    }

    [Fact]
    public void ModelBuilderConfiguresEntities()
    {
        // Arrange
        var options = GetInMemoryDbContextOptions();

        // Act
        using var context = new AppDbContext(options);
        var model = context.Model;
        var userEntity = model.FindEntityType(typeof(User));
        
        // Assert
        Assert.NotNull(userEntity);
    }
    
    [Fact]
    public void CanAddAndRetrieveToken()
    {
        // Arrange
        var options = GetInMemoryDbContextOptions();
        
        var token = new Token();
        token.SetId(1);
        token.SetTokenValue("TestToken");
        token.SetTokenRevoked(false);
        token.SetTokenExpired(false);
        token.SetTokenType(TokenType.Bearer);

        // Act
        using (var context = new AppDbContext(options))
        {
            context.Tokens.Add(token);
            context.SaveChanges();
        }

        // Assert
        using (var context = new AppDbContext(options))
        {
            var retrievedToken = context.Tokens.Find(token.Id);
            Assert.NotNull(retrievedToken);
            Assert.Equal("TestToken", retrievedToken.TokenValue);
        }
    }

    [Fact]
    public void CanRemoveToken()
    {
        // Arrange
        var options = GetInMemoryDbContextOptions();
        
        var token = new Token();
        token.SetId(1);
        token.SetTokenValue("TestToken");
        token.SetTokenRevoked(false);
        token.SetTokenExpired(false);
        token.SetTokenType(TokenType.Bearer);

        // Act
        using (var context = new AppDbContext(options))
        {
            context.Tokens.Add(token);
            context.SaveChanges();

            context.Tokens.Remove(token);
            context.SaveChanges();
        }

        // Assert
        using (var context = new AppDbContext(options))
        {
            var retrievedToken = context.Tokens.Find(token.Id);
            Assert.Null(retrievedToken);
        }
    }
}