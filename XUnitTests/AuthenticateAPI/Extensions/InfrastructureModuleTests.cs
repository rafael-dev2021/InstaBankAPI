using AuthenticateAPI.Context;
using AuthenticateAPI.Extensions;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace XUnitTests.AuthenticateAPI.Extensions;

public class InfrastructureModuleTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly GenerateKey _generateKey = new();

    public InfrastructureModuleTests()
    {
        var serviceCollection = new ServiceCollection();

        Environment.SetEnvironmentVariable("DB_PASSWORD", "TestPassword");
        Environment.SetEnvironmentVariable("SECRET_KEY", GenerateKey.GenerateHmac256Key());
        Environment.SetEnvironmentVariable("ISSUER", "http://localhost");
        Environment.SetEnvironmentVariable("AUDIENCE", "http://localhost");
        Environment.SetEnvironmentVariable("EXPIRES_TOKEN", "15");
        Environment.SetEnvironmentVariable("EXPIRES_REFRESHTOKEN", "30");

        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {
                    "ConnectionStrings:DefaultConnection",
                    "Server=(localdb)\\mssqllocaldb;Database=InMemoryDbForTesting;Trusted_Connection=True;"
                },
                { "Jwt:SecretKey", "SuperSecretKey12345" },
                { "Jwt:Issuer", "http://localhost" },
                { "Jwt:Audience", "http://localhost" },
                { "EXPIRES_TOKEN", "15" },
                { "EXPIRES_REFRESHTOKEN", "30" }
            }!)
            .Build();

        serviceCollection.AddInfrastructureModule();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void Test_DatabaseDependencyInjection_IsRegistered()
    {
        var dbContext = _serviceProvider.GetService<AppDbContext>();
        Assert.NotNull(dbContext);
    }

    [Fact]
    public void Test_FluentValidationDependencyInjection_IsRegistered()
    {
        var validator = _serviceProvider.GetService<IValidator<User>>();
        Assert.NotNull(validator);
    }

    [Fact]
    public void Test_DependencyInjectionRepositories_IsRegistered()
    {
        var authenticatedRepository = _serviceProvider.GetService<IAuthenticatedRepository>();
        var userRoleRepository = _serviceProvider.GetService<IUserRoleRepository>();
        Assert.NotNull(authenticatedRepository);
        Assert.NotNull(userRoleRepository);
    }
}