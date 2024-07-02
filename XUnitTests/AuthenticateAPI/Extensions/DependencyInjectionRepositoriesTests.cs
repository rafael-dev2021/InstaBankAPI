using AuthenticateAPI.Context;
using AuthenticateAPI.Extensions;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories;
using AuthenticateAPI.Repositories.Interfaces;
using AuthenticateAPI.Repositories.Strategies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace XUnitTests.AuthenticateAPI.Extensions;

public class DependencyInjectionRepositoriesTests
{
    private readonly IServiceProvider _serviceProvider;

    public DependencyInjectionRepositoriesTests()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("TestDatabase"));

        serviceCollection.AddLogging(); 

        serviceCollection.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
        
        serviceCollection.AddDistributedMemoryCache();
        serviceCollection.AddDependencyInjectionRepositories();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void Test_IAuthenticatedRepository_IsRegistered()
    {
        var service = _serviceProvider.GetService<IAuthenticatedRepository>();
        Assert.NotNull(service);
        Assert.IsType<AuthenticatedRepository>(service);
    }

    [Fact]
    public void Test_IUserRoleRepository_IsRegistered()
    {
        var service = _serviceProvider.GetService<IUserRoleRepository>();
        Assert.NotNull(service);
        Assert.IsType<UserRoleRepository>(service);
    }

    [Fact]
    public void Test_IAuthenticatedStrategy_IsRegistered()
    {
        var service = _serviceProvider.GetService<IAuthenticatedStrategy>();
        Assert.NotNull(service);
        Assert.IsType<AuthenticatedStrategy>(service);
    }

    [Fact]
    public void Test_IRegisterStrategy_IsRegistered()
    {
        var service = _serviceProvider.GetService<IRegisterStrategy>();
        Assert.NotNull(service);
        Assert.IsType<RegisterStrategy>(service);
    }

    [Fact]
    public void Test_IUpdateProfileStrategy_IsRegistered()
    {
        var service = _serviceProvider.GetService<IUpdateProfileStrategy>();
        Assert.NotNull(service);
        Assert.IsType<UpdateProfileStrategy>(service);
    }
}