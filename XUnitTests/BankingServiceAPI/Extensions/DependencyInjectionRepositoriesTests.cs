using BankingServiceAPI.Algorithms;
using BankingServiceAPI.Algorithms.Interfaces;
using BankingServiceAPI.Context;
using BankingServiceAPI.Extensions;
using BankingServiceAPI.Repositories;
using BankingServiceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace XUnitTests.BankingServiceAPI.Extensions;

public class DependencyInjectionRepositoriesTests
{
    private readonly IServiceProvider _serviceProvider;

    public DependencyInjectionRepositoriesTests()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("TestDatabase"));

        serviceCollection.AddLogging(); 

        serviceCollection.AddDependencyInjectionRepositories();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void Test_IBankAccountRepository_IsRegistered()
    {
        var service = _serviceProvider.GetService<IBankAccountRepository>();
        Assert.NotNull(service);
        Assert.IsType<BankAccountRepository>(service);
    }

    [Fact]
    public void Test_IBankTransactionRepository_IsRegistered()
    {
        var service = _serviceProvider.GetService<IBankTransactionRepository>();
        Assert.NotNull(service);
        Assert.IsType<BankTransactionRepository>(service);
    }

    [Fact]
    public void Test_IAccountNumberGenerator_IsRegistered()
    {
        var service = _serviceProvider.GetService<IAccountNumberGenerator>();
        Assert.NotNull(service);
        Assert.IsType<AccountNumberGenerator>(service);
    }
}