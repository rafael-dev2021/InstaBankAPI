using BankingServiceAPI.Context;
using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace XUnitTests.BankingServiceAPI.Repositories;

public class BankTransactionRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly BankTransactionRepository _bankTransactionRepository;

    public BankTransactionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _bankTransactionRepository = new BankTransactionRepository(_context);
    }

    [Fact]
    public async Task GetEntitiesAsync_Should_Return_All_BankTransactions()
    {
        // Arrange
        var user = new User();
        user.SetId("123");
        user.SetName("John");
        user.SetLastName("Doe");
        user.SetEmail("john.doe@example.com");
        user.SetPhoneNumber("123456789");
        user.SetCpf("123.456.789-00");
        user.SetRole("Admin");

        var bankAccount1 = new BankAccount();
        bankAccount1.SetId(1);
        bankAccount1.SetAccountNumber(123456);
        bankAccount1.SetAgency(1234);
        bankAccount1.SetBalance(1000);
        bankAccount1.SetAccountType(AccountType.Savings);
        bankAccount1.SetUser(user);

        var bankAccount2 = new BankAccount();
        bankAccount2.SetId(2);
        bankAccount2.SetAccountNumber(654321);
        bankAccount2.SetAgency(4321);
        bankAccount2.SetBalance(500);
        bankAccount2.SetAccountType(AccountType.Current);
        bankAccount2.SetUser(user);

        var bankTransaction = new BankTransaction();
        bankTransaction.SetId(1);
        bankTransaction.SetAccountOriginId(1);
        bankTransaction.SetAccountOrigin(bankAccount1);
        bankTransaction.SetAccountDestinationId(1);
        bankTransaction.SetAccountDestination(bankAccount2);
        bankTransaction.SetAmount(100);
        bankTransaction.SetTransferDate(DateTime.UtcNow);

        var bankTransaction2 = new BankTransaction();
        bankTransaction2.SetId(2);
        bankTransaction.SetAccountOriginId(2);
        bankTransaction2.SetAccountOrigin(bankAccount2);
        bankTransaction.SetAccountDestinationId(2);
        bankTransaction2.SetAccountDestination(bankAccount1);
        bankTransaction2.SetAmount(200);
        bankTransaction2.SetTransferDate(DateTime.UtcNow);

        await _context.Users.AddAsync(user);
        await _context.BankAccounts.AddRangeAsync(bankAccount1, bankAccount2);
        await _context.BankTransactions.AddRangeAsync(bankTransaction, bankTransaction2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bankTransactionRepository.GetEntitiesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetEntityByIdAsync_Should_Return_BankTransaction_By_Id()
    {
        // Arrange
        var user = new User();
        user.SetId("123");
        user.SetName("John");
        user.SetLastName("Doe");
        user.SetEmail("john.doe@example.com");
        user.SetPhoneNumber("123456789");
        user.SetCpf("123.456.789-00");
        user.SetRole("Admin");

        var bankAccount1 = new BankAccount();
        bankAccount1.SetId(1);
        bankAccount1.SetAccountNumber(123456);
        bankAccount1.SetAgency(1234);
        bankAccount1.SetBalance(1000);
        bankAccount1.SetAccountType(AccountType.Savings);
        bankAccount1.SetUser(user);

        var bankAccount2 = new BankAccount();
        bankAccount2.SetId(2);
        bankAccount2.SetAccountNumber(654321);
        bankAccount2.SetAgency(4321);
        bankAccount2.SetBalance(500);
        bankAccount2.SetAccountType(AccountType.Current);
        bankAccount2.SetUser(user);

        var bankTransaction = new BankTransaction();
        bankTransaction.SetId(1);
        bankTransaction.SetAccountOrigin(bankAccount1);
        bankTransaction.SetAccountDestination(bankAccount2);
        bankTransaction.SetAmount(100);
        bankTransaction.SetTransferDate(DateTime.UtcNow);

        await _context.Users.AddAsync(user);
        await _context.BankAccounts.AddRangeAsync(bankAccount1, bankAccount2);
        await _context.BankTransactions.AddAsync(bankTransaction);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bankTransactionRepository.GetEntityByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }


    [Fact]
    public async Task CreateEntityAsync_Should_Create_BankTransaction()
    {
        // Arrange
        var user = new User();
        user.SetId("123");
        user.SetName("John");
        user.SetLastName("Doe");
        user.SetEmail("john.doe@example.com");
        user.SetPhoneNumber("123456789");
        user.SetCpf("123.456.789-00");
        user.SetRole("Admin");

        var bankAccount1 = new BankAccount();
        bankAccount1.SetId(1);
        bankAccount1.SetAccountNumber(123456);
        bankAccount1.SetAgency(1234);
        bankAccount1.SetBalance(1000);
        bankAccount1.SetAccountType(AccountType.Savings);
        bankAccount1.SetUser(user);

        var bankAccount2 = new BankAccount();
        bankAccount2.SetId(2);
        bankAccount2.SetAccountNumber(654321);
        bankAccount2.SetAgency(4321);
        bankAccount2.SetBalance(500);
        bankAccount2.SetAccountType(AccountType.Current);
        bankAccount2.SetUser(user);

        var bankTransaction = new BankTransaction();
        bankTransaction.SetId(1);
        bankTransaction.SetAccountOrigin(bankAccount1);
        bankTransaction.SetAccountDestination(bankAccount2);
        bankTransaction.SetAmount(100);
        bankTransaction.SetTransferDate(DateTime.UtcNow);

        // Act
        var result = await _bankTransactionRepository.CreateEntityAsync(bankTransaction);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(100, result.Amount);
        Assert.Equal(bankAccount1, result.AccountOrigin);
        Assert.Equal(bankAccount2, result.AccountDestination);
    }

    [Fact]
    public async Task DeleteEntityAsync_Should_Delete_BankTransaction()
    {
        // Arrange
        var user = new User();
        user.SetId("123");
        user.SetName("John");
        user.SetLastName("Doe");
        user.SetEmail("john.doe@example.com");
        user.SetPhoneNumber("123456789");
        user.SetCpf("123.456.789-00");
        user.SetRole("Admin");

        var bankAccount1 = new BankAccount();
        bankAccount1.SetId(1);
        bankAccount1.SetAccountNumber(123456);
        bankAccount1.SetAgency(1234);
        bankAccount1.SetBalance(1000);
        bankAccount1.SetAccountType(AccountType.Savings);
        bankAccount1.SetUser(user);

        var bankAccount2 = new BankAccount();
        bankAccount2.SetId(2);
        bankAccount2.SetAccountNumber(654321);
        bankAccount2.SetAgency(4321);
        bankAccount2.SetBalance(500);
        bankAccount2.SetAccountType(AccountType.Current);
        bankAccount2.SetUser(user);

        var bankTransaction = new BankTransaction();
        bankTransaction.SetId(1);
        bankTransaction.SetAccountOrigin(bankAccount1);
        bankTransaction.SetAccountDestination(bankAccount2);
        bankTransaction.SetAmount(100);
        bankTransaction.SetTransferDate(DateTime.UtcNow);

        await _context.BankTransactions.AddAsync(bankTransaction);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bankTransactionRepository.DeleteEntityAsync(1);

        // Assert
        Assert.True(result);
        Assert.Null(await _bankTransactionRepository.GetEntityByIdAsync(1));
    }

    [Fact]
    public async Task DeleteEntityAsync_Should_Throw_Exception_If_Not_Found()
    {
        // Act & Assert
        await Assert.ThrowsAsync<GetIdNotFoundException>(() => _bankTransactionRepository.DeleteEntityAsync(1));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}