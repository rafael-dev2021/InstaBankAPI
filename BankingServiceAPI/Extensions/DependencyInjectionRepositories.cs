using BankingServiceAPI.Algorithms;
using BankingServiceAPI.Algorithms.Interfaces;
using BankingServiceAPI.Repositories;
using BankingServiceAPI.Repositories.Interfaces;

namespace BankingServiceAPI.Extensions;

public static class DependencyInjectionRepositories
{
    public static void AddDependencyInjectionRepositories(this IServiceCollection service)
    {
        service
            .AddScoped<IBankAccountRepository, BankAccountRepository>()
            .AddScoped<IBankTransactionRepository, BankTransactionRepository>()
            .AddScoped<IAccountNumberGenerator, AccountNumberGenerator>()
            .AddScoped<ITransferRepository, TransferRepository>()
            .AddScoped<IDepositRepository, DepositRepository>()
            .AddScoped<IWithdrawRepository, WithdrawRepository>()
            .AddScoped<ITransactionLogRepository, TransactionLogRepository>();
    }
}