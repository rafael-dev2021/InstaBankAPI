using BankingServiceAPI.Context;
using BankingServiceAPI.Extensions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace XUnitTests.BankingServiceAPI.Extensions;

public class InfrastructureModuleTests
{
    private readonly IServiceProvider _serviceProvider;

        public InfrastructureModuleTests()
        {
            var serviceCollection = new ServiceCollection();

            // Configure environment variables
            Environment.SetEnvironmentVariable("DB_PASSWORD", "TestPassword");
            Environment.SetEnvironmentVariable("SECRET_KEY", "SuperSecretKey12345");
            Environment.SetEnvironmentVariable("ISSUER", "http://localhost");
            Environment.SetEnvironmentVariable("AUDIENCE", "http://localhost");

            // Configure in-memory settings
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ConnectionStrings:DefaultConnection", "Server=(localdb)\\mssqllocaldb;Database=InMemoryDbForTesting;Trusted_Connection=True;" },
                    { "Jwt:SecretKey", "SuperSecretKey12345" },
                    { "Jwt:Issuer", "http://localhost" },
                    { "Jwt:Audience", "http://localhost" },
                }!)
                .Build();

            // Register configuration and other services
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddLogging();

            // Call the method to register all dependencies
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
            var validator = _serviceProvider.GetService<IValidator<BankAccount>>();
            Assert.NotNull(validator);
        }

        [Fact]
        public void Test_DependencyInjectionRepositories_IsRegistered()
        {
            var bankAccountRepository = _serviceProvider.GetService<IBankAccountRepository>();
            var bankTransactionRepository = _serviceProvider.GetService<IBankTransactionRepository>();
            Assert.NotNull(bankAccountRepository);
            Assert.NotNull(bankTransactionRepository);
        }
}