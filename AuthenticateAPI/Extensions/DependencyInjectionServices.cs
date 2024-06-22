using AuthenticateAPI.Security;
using AuthenticateAPI.Services;

namespace AuthenticateAPI.Extensions;

public static class DependencyInjectionServices
{
    public static void AddDependencyInjectionService(this IServiceCollection service)
    {
        service.AddScoped<ITokenManagerService, TokenManagerService>();
        service.AddTransient<ITokenService, TokenService>();
        service.AddTransient<IAuthenticateService, AuthenticateService>();
    }
}