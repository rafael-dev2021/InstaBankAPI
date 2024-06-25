using BankingServiceAPI.Services;
using BankingServiceAPI.Services.Interfaces;

namespace BankingServiceAPI.Extensions;

public static class DependencyInjectionServices
{
    public static void AddDependencyInjectionServices(this IServiceCollection service)
    {
        service.AddScoped<IBankAccountDtoService, BankAccountDtoService>();
        service.AddScoped<IUserContextService, UserContextService>();
        service.AddScoped<ITransferService, TransferService>();
    }
}