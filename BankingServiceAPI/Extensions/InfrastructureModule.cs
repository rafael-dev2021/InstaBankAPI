namespace BankingServiceAPI.Extensions;

public static class InfrastructureModule
{
    public static void AddInfrastructureModule(this IServiceCollection service)
    {
        service.AddDatabaseDependencyInjection();
        service.AddFluentValidationDependencyInjection();
        service.AddDependencyInjectionRepositories();
    }
}