using AuthenticateAPI.Context;
using AuthenticateAPI.Extensions;
using AuthenticateAPI.Repositories;
using AuthenticateAPI.Repositories.Interfaces;
using AuthenticateAPI.Repositories.Strategies;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace XUnitTests.AuthenticateAPI.Extensions;

public class DependencyInjectionRepositoriesTests
{
    private readonly IServiceProvider _serviceProvider;

    public DependencyInjectionRepositoriesTests()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("TestDatabase"));

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

    [Fact]
    public void Test_IHttpContextAccessor_IsRegisteredAsSingleton()
    {
        var service1 = _serviceProvider.GetService<IHttpContextAccessor>();
        var service2 = _serviceProvider.GetService<IHttpContextAccessor>();
        Assert.NotNull(service1);
        Assert.NotNull(service2);
        Assert.Same(service1, service2);
    }

    [Fact]
    public async Task Test_AdminPolicy_IsRegistered()
    {
        var policyProvider = _serviceProvider.GetService<IAuthorizationPolicyProvider>();
        Assert.NotNull(policyProvider);

        var policy = await policyProvider.GetPolicyAsync("Admin");
        Assert.NotNull(policy);
        Assert.Single(policy.Requirements);
        Assert.IsType<RolesAuthorizationRequirement>(policy.Requirements[0]);

        var rolesRequirement = policy.Requirements[0] as RolesAuthorizationRequirement;
        Assert.Contains("Admin", rolesRequirement!.AllowedRoles);
    }

    [Fact]
    public void Test_AntiforgeryOptions_HeaderName_IsConfigured()
    {
        var antiforgeryOptions = _serviceProvider.GetService<IAntiforgery>();
        Assert.NotNull(antiforgeryOptions);

        var options = _serviceProvider.GetRequiredService<IOptions<AntiforgeryOptions>>().Value;
        Assert.Equal("X-CSRF-TOKEN", options.HeaderName);
    }

    [Fact]
    public void Test_HttpsRedirectionOptions_HttpsPort_IsNull()
    {
        var options = _serviceProvider.GetRequiredService<IOptions<HttpsRedirectionOptions>>().Value;
        Assert.Null(options.HttpsPort);
    }

    [Fact]
    public void Test_MvcOptions_SecurityHeadersFilter_IsRegistered()
    {
        var mvcOptions = _serviceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;
        var filters = mvcOptions.Filters;

        var securityHeadersFilter =
            filters.FirstOrDefault(filter => filter.GetType() == typeof(SecurityHeadersAttribute));
        Assert.NotNull(securityHeadersFilter);
    }

    [Fact]
    public void Test_PasswordOptions_AreConfigured()
    {
        var passwordOptions = _serviceProvider.GetRequiredService<IOptions<PasswordOptions>>().Value;

        Assert.True(passwordOptions.RequireDigit);
        Assert.True(passwordOptions.RequireLowercase);
        Assert.True(passwordOptions.RequireUppercase);
        Assert.True(passwordOptions.RequireNonAlphanumeric);
        Assert.Equal(8, passwordOptions.RequiredLength);
        Assert.Equal(6, passwordOptions.RequiredUniqueChars);
    }
}