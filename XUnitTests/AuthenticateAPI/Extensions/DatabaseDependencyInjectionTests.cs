using AuthenticateAPI.Context;
using AuthenticateAPI.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace XUnitTests.AuthenticateAPI.Extensions;

public class DatabaseDependencyInjectionTests
{
    [Fact]
    public void AddDatabaseDependencyInjection_ShouldRegisterDbContext()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"ConnectionStrings:DefaultConnection", "InMemoryDbForTesting"}
            }!)
            .Build();

        // Act
        services.AddDatabaseDependencyInjection(configuration);
        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetService<AppDbContext>();

        // Assert
        Assert.NotNull(dbContext);
        Assert.IsType<AppDbContext>(dbContext);
    }
}