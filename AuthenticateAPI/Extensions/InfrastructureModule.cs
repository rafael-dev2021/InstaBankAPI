namespace AuthenticateAPI.Extensions;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructureModule(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddDatabaseDependencyInjection(configuration);
        service.AddFluentValidationDependencyInjection();

        return service;
    }
}