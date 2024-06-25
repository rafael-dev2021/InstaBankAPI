using BankingServiceAPI.Algorithms;
using BankingServiceAPI.Algorithms.Interfaces;
using BankingServiceAPI.Repositories;
using BankingServiceAPI.Repositories.Interfaces;

namespace BankingServiceAPI.Extensions;

public static class DependencyInjectionRepositories
{
    public static void AddDependencyInjectionRepositories(this IServiceCollection service)
    {
        service.AddScoped<IBankAccountRepository, BankAccountRepository>();
        service.AddScoped<IBankTransactionRepository, BankTransactionRepository>();
        service.AddScoped<IAccountNumberGenerator, AccountNumberGenerator>();
        service.AddScoped<ITransferRepository, TransferRepository>();
    }
}