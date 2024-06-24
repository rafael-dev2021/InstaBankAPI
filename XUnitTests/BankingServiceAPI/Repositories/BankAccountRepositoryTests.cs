using BankingServiceAPI.Algorithms.Interfaces;
using BankingServiceAPI.Context;
using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace XUnitTests.BankingServiceAPI.Repositories;

public class BankAccountRepositoryTests : IDisposable
{
    private readonly Mock<IAccountNumberGenerator> _accountNumberGeneratorMock;
    private readonly AppDbContext _context;
    private readonly BankAccountRepository _bankAccountRepository;
    
    public BankAccountRepositoryTests()
    {
        _accountNumberGeneratorMock = new Mock<IAccountNumberGenerator>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _bankAccountRepository = new BankAccountRepository(_context, _accountNumberGeneratorMock.Object);
    }

    [Fact]
    public async Task GetEntitiesAsync_Should_Return_All_BankAccounts()
    {
        // Arrange
        var user = new User();
        user.SetId("123");
        user.SetName("John");
        user.SetLastName("John");
        user.SetEmail("john.doe@example.com");
        user.SetPhoneNumber("123456789");
        user.SetCpf("123.456.789-00");
        user.SetRole("Admin");

        var user2 = new User();
        user2.SetId("1234");
        user2.SetName("John");
        user2.SetLastName("John");
        user2.SetEmail("john.doe2@example.com");
        user2.SetPhoneNumber("123456784");
        user2.SetCpf("123.456.789-01");
        user2.SetRole("User");

        var bankAccount = new BankAccount();
        bankAccount.SetId(1);
        bankAccount.SetAccountNumber(123456);
        bankAccount.SetAgency(1234);
        bankAccount.SetBalance(100);
        bankAccount.SetAccountType(AccountType.Savings);
        bankAccount.SetUser(user);

        var bankAccount2 = new BankAccount();
        bankAccount2.SetId(2);
        bankAccount2.SetAccountNumber(123456);
        bankAccount2.SetAgency(1234);
        bankAccount2.SetBalance(100);
        bankAccount2.SetAccountType(AccountType.Savings);
        bankAccount2.SetUser(user2);

        var bankAccounts = new List<BankAccount> { bankAccount, bankAccount2 };
        await _context.BankAccounts.AddRangeAsync(bankAccounts);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bankAccountRepository.GetEntitiesAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetEntityByIdAsync_Should_Return_BankAccount_By_Id()
    {
        // Arrange
        var user = new User();
        user.SetId("123");
        user.SetName("John");
        user.SetLastName("John");
        user.SetEmail("john.doe@example.com");
        user.SetPhoneNumber("123456789");
        user.SetCpf("123.456.789-00");
        user.SetRole("Admin");

        var bankAccount = new BankAccount();
        bankAccount.SetId(1);
        bankAccount.SetAccountNumber(123456);
        bankAccount.SetAgency(1234);
        bankAccount.SetBalance(100);
        bankAccount.SetAccountType(AccountType.Savings);
        bankAccount.SetUser(user);

        await _context.BankAccounts.AddAsync(bankAccount);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bankAccountRepository.GetEntityByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task CreateEntityAsync_Should_Create_BankAccount()
    {
        // Arrange
        var user = new User();
        user.SetId("1");
        user.SetName("John");
        user.SetLastName("Doe");
        user.SetEmail("john.doe@example.com");
        user.SetPhoneNumber("123456789");
        user.SetCpf("123.456.789-00");
        user.SetRole("Admin");

        var bankAccount = new BankAccount();
        bankAccount.SetId(1);
        bankAccount.SetBalance(1000);
        bankAccount.SetAccountType(AccountType.Savings);
        bankAccount.SetUser(user);

        _accountNumberGeneratorMock.Setup(x => x.GenerateAccountNumber()).Returns(123456);
        _accountNumberGeneratorMock.Setup(x => x.GenerateAgencyNumber()).Returns(456);

        // Act
        var result = await _bankAccountRepository.CreateEntityAsync(bankAccount);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(123456, result.AccountNumber);
        Assert.Equal(456, result.Agency);
        Assert.Equal(1000, result.Balance);
        Assert.Equal(AccountType.Savings, result.AccountType);
        Assert.Equal(user, result.User);
    }

    [Fact]
    public async Task DeleteEntityAsync_Should_Delete_BankAccount()
    {
        // Arrange
        var user = new User();
        user.SetId("123");
        user.SetName("John");
        user.SetLastName("John");
        user.SetEmail("john.doe@example.com");
        user.SetPhoneNumber("123456789");
        user.SetCpf("123.456.789-00");
        user.SetRole("Admin");

        var bankAccount = new BankAccount();
        bankAccount.SetId(1);
        bankAccount.SetAccountNumber(123456);
        bankAccount.SetAgency(1234);
        bankAccount.SetBalance(100);
        bankAccount.SetAccountType(AccountType.Savings);
        bankAccount.SetUser(user);

        await _context.BankAccounts.AddAsync(bankAccount);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bankAccountRepository.DeleteEntityAsync(1);

        // Assert
        Assert.True(result);
        Assert.Null(await _bankAccountRepository.GetEntityByIdAsync(1));
    }

    [Fact]
    public async Task DeleteEntityAsync_Should_Throw_Exception_If_Not_Found()
    {
        // Act & Assert
        await Assert.ThrowsAsync<GetIdNotFoundException>(() => _bankAccountRepository.DeleteEntityAsync(1));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
