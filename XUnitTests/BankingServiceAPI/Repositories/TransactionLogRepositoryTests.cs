using BankingServiceAPI.Context;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace XUnitTests.BankingServiceAPI.Repositories;

public class TransactionLogRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly TransactionLogRepository _repository;

    public TransactionLogRepositoryTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(dbContextOptions);
        _repository = new TransactionLogRepository(_context);
    }


    [Fact]
    public async Task GetAllTransactionLogsAsync_ShouldReturnAllTransactionLogs()
    {
        // Arrange
        var bankTransaction1 = new BankTransaction();
        bankTransaction1.SetId(1);

        var details1 = new TransactionDetails();
        details1.Configure(
            transactionStatus: "Completed",
            description: "Test Transaction 1",
            remarks: "No remarks",
            transactionReference: "Ref123",
            channel: "Online",
            errorDetails: "No errors"
        );

        var audit1 = new TransactionAudit();
        audit1.Configure(
            initiatedBy: "User1",
            approvedBy: "Admin1",
            ipAddress: "127.0.0.1",
            deviceId: "Device1",
            location: "Location1",
            timestamp: DateTime.UtcNow
        );

        var log1 = new TransactionLog();
        log1.Configure(
            bankTransactionId: 1,
            transactionType: "Type1",
            amount: 100,
            accountOriginId: 1,
            accountDestinationId: null,
            transactionDate: DateTime.UtcNow,
            details: details1,
            audit: audit1
        );
        log1.SetBankTransaction(bankTransaction1);

        var bankTransaction2 = new BankTransaction();
        bankTransaction2.SetId(2);

        var details2 = new TransactionDetails();
        details2.Configure(
            transactionStatus: "Completed",
            description: "Test Transaction 2",
            remarks: "No remarks",
            transactionReference: "Ref124",
            channel: "Online",
            errorDetails: "No errors"
        );

        var audit2 = new TransactionAudit();
        audit2.Configure(
            initiatedBy: "User2",
            approvedBy: "Admin2",
            ipAddress: "127.0.0.1",
            deviceId: "Device2",
            location: "Location2",
            timestamp: DateTime.UtcNow
        );

        var log2 = new TransactionLog();
        log2.Configure(
            bankTransactionId: 2,
            transactionType: "Type2",
            amount: 200,
            accountOriginId: 2,
            accountDestinationId: null,
            transactionDate: DateTime.UtcNow,
            details: details2,
            audit: audit2
        );
        log2.SetBankTransaction(bankTransaction2);

        await _context.TransactionLogs.AddRangeAsync(log1, log2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllTransactionLogsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetTransactionLogByIdAsync_ShouldReturnTransactionLog_WhenIdExists()
    {
        // Arrange
        var bankTransaction = new BankTransaction();
        bankTransaction.SetId(1);
        
        var details = new TransactionDetails();
        details.Configure(
            transactionStatus: "Completed",
            description: "Test Transaction",
            remarks: "No remarks",
            transactionReference: "Ref123",
            channel: "Online",
            errorDetails: "No errors"
        );

        var audit = new TransactionAudit();
        audit.Configure(
            initiatedBy: "User1",
            approvedBy: "Admin1",
            ipAddress: "127.0.0.1",
            deviceId: "Device1",
            location: "Location1",
            timestamp: DateTime.UtcNow
        );

        var log = new TransactionLog();
        log.Configure(
            bankTransactionId: 1,
            transactionType: "Type1",
            amount: 100,
            accountOriginId: 1,
            accountDestinationId: null,
            transactionDate: DateTime.UtcNow,
            details: details,
            audit: audit
        );
        log.SetBankTransaction(bankTransaction);

        await _context.TransactionLogs.AddAsync(log);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTransactionLogByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetTransactionLogByIdAsync_ShouldReturnNull_WhenIdDoesNotExist()
    {
        // Act
        var result = await _repository.GetTransactionLogByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateTransactionLogAsync_ShouldAddTransactionLog()
    {
        // Arrange
        var details = new TransactionDetails();
        details.Configure(
            transactionStatus: "Completed",
            description: "Test Transaction",
            remarks: "No remarks",
            transactionReference: "Ref123",
            channel: "Online",
            errorDetails: "No errors"
        );

        var audit = new TransactionAudit();
        audit.Configure(
            initiatedBy: "User1",
            approvedBy: "Admin1",
            ipAddress: "127.0.0.1",
            deviceId: "Device1",
            location: "Location1",
            timestamp: DateTime.UtcNow
        );

        var log = new TransactionLog();
        log.Configure(
            bankTransactionId: 1,
            transactionType: "Type1",
            amount: 100,
            accountOriginId: 1,
            accountDestinationId: null,
            transactionDate: DateTime.UtcNow,
            details: details,
            audit: audit
        );

        // Act
        var result = await _repository.CreateTransactionLogAsync(log);
        var savedLog = await _context.TransactionLogs.FindAsync(result.Id);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(savedLog);
        Assert.Equal(1, savedLog.Id);
    }

    [Fact]
    public async Task DeleteTransactionLogAsync_ShouldReturnTrue_WhenLogIsDeleted()
    {
        // Arrange
        var details = new TransactionDetails();
        details.Configure(
            transactionStatus: "Completed",
            description: "Test Transaction",
            remarks: "No remarks",
            transactionReference: "Ref123",
            channel: "Online",
            errorDetails: "No errors"
        );

        var audit = new TransactionAudit();
        audit.Configure(
            initiatedBy: "User1",
            approvedBy: "Admin1",
            ipAddress: "127.0.0.1",
            deviceId: "Device1",
            location: "Location1",
            timestamp: DateTime.UtcNow
        );

        var log = new TransactionLog();
        log.Configure(
            bankTransactionId: 1,
            transactionType: "Type1",
            amount: 100,
            accountOriginId: 1,
            accountDestinationId: null,
            transactionDate: DateTime.UtcNow,
            details: details,
            audit: audit
        );

        await _context.TransactionLogs.AddAsync(log);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteTransactionLogAsync(1);

        // Assert
        Assert.True(result);
        var deletedLog = await _context.TransactionLogs.FindAsync(1);
        Assert.Null(deletedLog);
    }

    [Fact]
    public async Task DeleteTransactionLogAsync_ShouldReturnFalse_WhenLogDoesNotExist()
    {
        // Act
        var result = await _repository.DeleteTransactionLogAsync(1);

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}