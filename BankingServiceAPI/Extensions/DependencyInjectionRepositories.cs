using BankingServiceAPI.Repositories;
using BankingServiceAPI.Repositories.Interfaces;

namespace BankingServiceAPI.Extensions;

public static class DependencyInjectionRepositories
{
    public static void AddDependencyInjectionRepositories(this IServiceCollection service)
    {
        service.AddScoped<IBaseEntityRepository, IBaseEntityRepository>();
        service.AddScoped<IIndividualAccountRepository, IndividualAccountRepository>();
        service.AddScoped<ICorporateAccountRepository, CorporateAccountRepository>();
        service.AddScoped<IAddressRepository, AddressRepository>();
    }
}