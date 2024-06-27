using BankingServiceAPI.Algorithms;
using BankingServiceAPI.Context;
using BankingServiceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace XUnitTests.BankingServiceAPI.Algorithms;

public class AccountNumberGeneratorTests
{
    private static DbContextOptions<AppDbContext> CreateNewDbContextOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GenerateAccountNumberAsync_Should_Return_Unique_Account_Number()
    {
        // Arrange
        var dbContextOptions = CreateNewDbContextOptions();

        await using var context = new AppDbContext(dbContextOptions);
        var generator = new AccountNumberGenerator(context);

        var existingAccount = new BankAccount();
        existingAccount.SetAccountNumber(123456);
        existingAccount.SetAgency(123);

        await context.BankAccounts.AddAsync(existingAccount);
        await context.SaveChangesAsync();

        // Act
        var accountNumber = await generator.GenerateAccountNumberAsync();

        // Assert
        Assert.NotEqual(existingAccount.AccountNumber, accountNumber);

        var isUnique = !await context.BankAccounts.AnyAsync(x => x.AccountNumber == accountNumber);
        Assert.True(isUnique);
    }

    [Fact]
    public void GenerateAgencyNumberAsync_Should_Return_Valid_Agency_Number()
    {
        // Arrange
        var dbContextOptions = CreateNewDbContextOptions();

        using var context = new AppDbContext(dbContextOptions);
        var generator = new AccountNumberGenerator(context);

        // Act
        var agencyNumber = generator.GenerateAgencyNumberAsync().Result;

        // Assert
        Assert.InRange(agencyNumber, 1000, 9999);
    }
}