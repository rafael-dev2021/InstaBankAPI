using AuthenticateAPI.Repositories;
using AuthenticateAPI.Repositories.Interfaces;
using AuthenticateAPI.Repositories.Strategies;

namespace AuthenticateAPI.Extensions;

public static class DependencyInjectionRepositories
{
    public static void AddDependencyInjectionRepositories(this IServiceCollection service)
    {
        service.AddScoped<IAuthenticatedRepository, AuthenticatedRepository>();
        service.AddScoped<IUserRoleRepository, UserRoleRepository>();
        service.AddScoped<IAuthenticatedStrategy, AuthenticatedStrategy>();
        service.AddScoped<IRegisterStrategy, RegisterStrategy>();
        service.AddScoped<IUpdateProfileStrategy, UpdateProfileStrategy>();
    }
}