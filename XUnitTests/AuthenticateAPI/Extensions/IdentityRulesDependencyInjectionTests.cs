using AuthenticateAPI.Context;
using AuthenticateAPI.Extensions;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
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

public class IdentityRulesDependencyInjectionTests
{
    private readonly IServiceProvider _serviceProvider;

    public IdentityRulesDependencyInjectionTests()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("TestDatabase"));

        serviceCollection.AddLogging(); 

        serviceCollection.AddIdentityRulesDependencyInjection();

        _serviceProvider = serviceCollection.BuildServiceProvider();
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
    
    [Fact]
    public void Test_ApplicationCookieOptions_AreConfigured()
    {
        var optionsSnapshot = _serviceProvider.GetRequiredService<IOptionsSnapshot<CookieAuthenticationOptions>>();
        var options = optionsSnapshot.Get(IdentityConstants.ApplicationScheme);

        Assert.True(options.Cookie.HttpOnly);
        Assert.Equal(CookieSecurePolicy.Always, options.Cookie.SecurePolicy);
        Assert.Equal(SameSiteMode.Strict, options.Cookie.SameSite);
        Assert.True(options.Cookie.IsEssential);
        Assert.Equal("/v1/auth/Login", options.LoginPath);
        Assert.Equal("/v1/auth/Logout", options.LogoutPath);
        Assert.Equal("/v1/auth/AccessDenied", options.AccessDeniedPath);
        Assert.True(options.SlidingExpiration);
        Assert.Equal(TimeSpan.FromMinutes(30), options.ExpireTimeSpan);
    }


}