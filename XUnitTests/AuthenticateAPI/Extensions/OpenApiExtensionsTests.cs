using AuthenticateAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace XUnitTests.AuthenticateAPI.Extensions;

public class OpenApiExtensionsTests
{
    private readonly IServiceProvider _serviceProvider;

    public OpenApiExtensionsTests()
    {
        var services = new ServiceCollection();

        Environment.SetEnvironmentVariable("OPENAPI_URL", "https://example.com");

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "OpenApi:Url", "https://example.com" }
            }!)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        services.AddLogging();
        services.AddOpenApiExtensions();

        _serviceProvider = services.BuildServiceProvider();
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
    public async Task Test_CorsPolicy_IsRegistered()
    {
        // Arrange
        var corsPolicyProvider = _serviceProvider.GetService<ICorsPolicyProvider>();
        Assert.NotNull(corsPolicyProvider);

        var httpContext = new DefaultHttpContext();

        // Act
        var corsPolicy = await corsPolicyProvider.GetPolicyAsync(httpContext, "CorsPolicy");

        // Assert
        Assert.NotNull(corsPolicy);

        var expectedOrigins = new[] { "https://localhost:7074", "http://localhost:5082" };
        Assert.Equal(expectedOrigins, corsPolicy.Origins);

        var expectedMethods = new[] { "GET", "POST", "PUT", "DELETE" };
        Assert.Equal(expectedMethods, corsPolicy.Methods);

        var expectedHeaders = new[] { "Authorization", "Content-Type" };
        Assert.Equal(expectedHeaders, corsPolicy.Headers);

        Assert.False(corsPolicy.AllowAnyOrigin);
        Assert.False(corsPolicy.AllowAnyMethod);
        Assert.False(corsPolicy.AllowAnyHeader);
    }

    [Fact]
    public async Task Test_ApiScopePolicy_IsRegistered()
    {
        var policyProvider = _serviceProvider.GetService<IAuthorizationPolicyProvider>();
        Assert.NotNull(policyProvider);

        var policy = await policyProvider.GetPolicyAsync("ApiScope");
        Assert.NotNull(policy);
        Assert.Contains(policy.Requirements.OfType<ClaimsAuthorizationRequirement>(), r =>
            r.ClaimType == "scope" &&
            r.AllowedValues!.Contains("AuthenticateAPI"));
    }

    [Fact]
    public void Test_SwaggerGen_IsRegistered()
    {
        var swaggerGenOptions = _serviceProvider.GetService<IOptions<SwaggerGenOptions>>();
        Assert.NotNull(swaggerGenOptions);

        var swaggerGenOptionsInstance = swaggerGenOptions.Value;

        Assert.Single(swaggerGenOptionsInstance.SwaggerGeneratorOptions.SwaggerDocs);
        Assert.Contains("v1", swaggerGenOptionsInstance.SwaggerGeneratorOptions.SwaggerDocs.Keys);

        var securityDefinition = swaggerGenOptionsInstance.SwaggerGeneratorOptions.SecuritySchemes;
        Assert.Contains("Bearer", securityDefinition);
        var bearerScheme = securityDefinition["Bearer"];
        Assert.Equal(SecuritySchemeType.ApiKey, bearerScheme.Type);
        Assert.Equal("Authorization", bearerScheme.Name);
        Assert.Equal(ParameterLocation.Header, bearerScheme.In);
    }
}