using BankingServiceAPI.Algorithms.Interfaces;
using BankingServiceAPI.Context;
using Microsoft.EntityFrameworkCore;

namespace BankingServiceAPI.Algorithms;

public class AccountNumberGenerator(AppDbContext appDbContext) : IAccountNumberGenerator
{
    private static readonly Random Random = new();

    public async Task<int> GenerateAgencyNumberAsync()
    {
        return await Task.FromResult(Random.Next(1000, 9999));
    }

    public async Task<int> GenerateAccountNumberAsync()
    {
        return await GenerateUniqueAccountNumberAsync();
    }

    private async Task<int> GenerateUniqueAccountNumberAsync()
    {
        while (true)
        {
            var accountNumber = Random.Next(10000, 999999);

            var exists = await appDbContext.BankAccounts.AnyAsync(x => x.AccountNumber == accountNumber);

            if (exists) continue;
            return accountNumber;
        }
    }
}