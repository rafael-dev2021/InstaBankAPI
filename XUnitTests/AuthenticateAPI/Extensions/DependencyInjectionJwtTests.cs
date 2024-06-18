using AuthenticateAPI.Extensions;
using AuthenticateAPI.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace XUnitTests.AuthenticateAPI.Extensions;

public class DependencyInjectionJwtTests
{
    private readonly IConfiguration _configuration;
    private readonly GenerateKey _generateKey = new();

    public DependencyInjectionJwtTests()
    {
        Environment.SetEnvironmentVariable("SECRET_KEY", GenerateKey.GenerateHmac256Key());
        Environment.SetEnvironmentVariable("ISSUER", "http://localhost");
        Environment.SetEnvironmentVariable("AUDIENCE", "http://localhost");
        Environment.SetEnvironmentVariable("EXPIRES_TOKEN", "15");
        Environment.SetEnvironmentVariable("EXPIRES_REFRESHTOKEN", "30");

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Jwt:SecretKey", "SuperSecretKey12345" },
                { "Jwt:Issuer", "http://localhost" },
                { "Jwt:Audience", "http://localhost" },
                { "EXPIRES_TOKEN", "15" },
                { "EXPIRES_REFRESHTOKEN", "30" }
            }!)
            .Build();
    }

    [Fact]
    public async Task AddDependencyInjectionJwt_ShouldConfigureJwtAuthentication()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(_configuration);

        // Act
        services.AddDependencyInjectionJwt();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var authService = serviceProvider.GetService<IAuthenticationSchemeProvider>();
        Assert.NotNull(authService);

        var tokenService = serviceProvider.GetService<ITokenService>();
        Assert.NotNull(tokenService);
        Assert.IsType<TokenService>(tokenService);

        var authServiceType = await authService.GetSchemeAsync(JwtBearerDefaults.AuthenticationScheme);
        Assert.NotNull(authServiceType);

        var optionsMonitor = serviceProvider.GetService<IOptionsMonitor<JwtBearerOptions>>();
        Assert.NotNull(optionsMonitor);

        var jwtOptions = optionsMonitor.Get(JwtBearerDefaults.AuthenticationScheme);
        Assert.NotNull(jwtOptions);
        Assert.Equal("http://localhost", jwtOptions.TokenValidationParameters.ValidIssuer);
        Assert.Equal("http://localhost", jwtOptions.TokenValidationParameters.ValidAudience);
        Assert.NotNull(jwtOptions.TokenValidationParameters.IssuerSigningKey);
    }

    [Fact]
    public void AddDependencyInjectionJwt_ShouldAddAuthorization()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(_configuration);

        // Act
        services.AddDependencyInjectionJwt();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var authorizationService = serviceProvider.GetService<IAuthorizationService>();
        Assert.NotNull(authorizationService);
    }
}