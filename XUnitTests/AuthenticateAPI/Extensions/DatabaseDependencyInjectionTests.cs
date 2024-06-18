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

        // Set the environment variable for the test
        Environment.SetEnvironmentVariable("DB_PASSWORD", "TestPassword");

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {
                    "ConnectionStrings:DefaultConnection",
                    "Server=(localdb)\\mssqllocaldb;Database=InMemoryDbForTesting;Trusted_Connection=True;"
                }
            }!)
            .Build();

        // Add the configuration to the service collection
        services.AddSingleton<IConfiguration>(configuration);

        // Act
        services.AddDatabaseDependencyInjection();
        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetService<AppDbContext>();

        // Assert
        Assert.NotNull(dbContext);
        Assert.IsType<AppDbContext>(dbContext);

        Environment.SetEnvironmentVariable("DB_PASSWORD", null);
    }
}