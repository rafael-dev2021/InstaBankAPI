using AuthenticateAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
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
        var corsPolicyProvider = _serviceProvider.GetService<ICorsPolicyProvider>();
        Assert.NotNull(corsPolicyProvider);

        var httpContext = new DefaultHttpContext();

        var corsPolicy = await corsPolicyProvider.GetPolicyAsync(httpContext, "CorsPolicy");
        Assert.NotNull(corsPolicy);
        Assert.True(corsPolicy.AllowAnyOrigin);
        Assert.True(corsPolicy.AllowAnyMethod);
        Assert.True(corsPolicy.AllowAnyHeader);
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