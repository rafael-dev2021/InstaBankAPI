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

    public InfrastructureModuleTests()
    {
        var serviceCollection = new ServiceCollection();

        // Set the environment variable for the test
        Environment.SetEnvironmentVariable("DB_PASSWORD", "TestPassword");

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"ConnectionStrings:DefaultConnection", "Server=(localdb)\\mssqllocaldb;Database=InMemoryDbForTesting;Trusted_Connection=True;"}
            }!)
            .Build();

        serviceCollection.AddInfrastructureModule(configuration);

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