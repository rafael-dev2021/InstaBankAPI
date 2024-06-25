using BankingServiceAPI.Services;
using BankingServiceAPI.Services.Interfaces;

namespace BankingServiceAPI.Extensions;

public static class DependencyInjectionServices
{
    public static void AddDependencyInjectionServices(this IServiceCollection service)
    {
        service
            .AddScoped<IBankAccountDtoService, BankAccountDtoService>()
            .AddScoped<IUserContextService, UserContextService>()
            .AddScoped<ITransferService, TransferService>()
            .AddScoped<IDepositService, DepositService>()
            .AddScoped<IWithdrawService, WithdrawService>();
    }
}