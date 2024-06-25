using BankingServiceAPI.Algorithms.Interfaces;
using BankingServiceAPI.Context;

namespace BankingServiceAPI.Algorithms;

public class AccountNumberGenerator(AppDbContext context) : IAccountNumberGenerator
{
    private static readonly Random Random = new();

    public int GenerateAccountNumber()
    {
        return GenerateAccountNumberRecursive();
    }

    public int GenerateAgencyNumber()
    {
        return Random.Next(1000, 9999);
    }

    private int GenerateAccountNumberRecursive()
    {
        var accountNumber = Random.Next(10000, 999999);

        return context.BankAccounts.Any(x => x.AccountNumber == accountNumber)
            ? GenerateAccountNumberRecursive()
            : accountNumber;
    }
}