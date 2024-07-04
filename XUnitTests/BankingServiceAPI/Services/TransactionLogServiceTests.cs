using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using BankingServiceAPI.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace XUnitTests.BankingServiceAPI.Services;

public class TransactionLogServiceTests
{
    private readonly Mock<ITransactionLogRepository> _transactionLogRepositoryMock;
    private readonly TransactionLogService _transactionLogService;

    public TransactionLogServiceTests()
    {
        _transactionLogRepositoryMock = new Mock<ITransactionLogRepository>();
        Mock<IHttpContextAccessor> httpContextAccessorMock = new();
        _transactionLogService =
            new TransactionLogService(_transactionLogRepositoryMock.Object, httpContextAccessorMock.Object);

        var context = new DefaultHttpContext();
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);
    }

    [Fact]
    public async Task CreateTransactionDetailsAsync_ShouldCreateTransactionDetails()
    {
        // Arrange
        var bankTransaction = new BankTransaction();
        bankTransaction.SetId(1);
        bankTransaction.SetAmount(1000);
        
        var accountOrigin = new BankAccount();
        
        var user = new User();
        user.SetId("User1");
        
        accountOrigin.SetUser(user);
        bankTransaction.SetAccountOrigin(accountOrigin);

        // Act
        var result = await _transactionLogService.CreateTransactionDetailsAsync(bankTransaction);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Completed", result.TransactionStatus);
        Assert.Equal("BankTransaction operation", result.Description);
        Assert.Equal("Successfully completed", result.Remarks);
        Assert.Equal("Internet Banking", result.Channel);
    }

    [Fact]
    public async Task CreateTransactionAuditAsync_ShouldCreateTransactionAudit()
    {
        // Arrange
        var bankTransaction = new BankTransaction();
        bankTransaction.SetId(1);
        bankTransaction.SetAmount(1000);
        
        var accountOrigin = new BankAccount();
        
        var user = new User();
        user.SetId("User1");
        
        accountOrigin.SetUser(user);
        bankTransaction.SetAccountOrigin(accountOrigin);

        // Act
        var result = await _transactionLogService.CreateTransactionAuditAsync(bankTransaction);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("User1", result.InitiatedBy);
        Assert.Equal("System", result.ApprovedBy);
        Assert.NotNull(result.IpAddress);
        Assert.NotNull(result.DeviceId);
        Assert.Equal("Brazil", result.Location);
    }

    [Fact]
    public async Task CreateTransactionLogAsync_ShouldCreateTransactionLog()
    {
        // Arrange
        var bankTransaction = new BankTransaction();
        bankTransaction.SetId(1);
        bankTransaction.SetAmount(1000);
        
        var accountOrigin = new BankAccount();
        
        var user = new User();
        user.SetId("User1");
        
        accountOrigin.SetUser(user);
        bankTransaction.SetAccountOrigin(accountOrigin);

        var transactionDetails = new TransactionDetails();
        transactionDetails.Configure("Completed", "BankTransaction operation", "Successfully completed", "Ref123",
            "Internet Banking", null);

        var transactionAudit = new TransactionAudit();
        transactionAudit.Configure("User1", "System", "127.0.0.1", "Device1", "Brazil", DateTime.Now);

        var createdLog = new TransactionLog();
        createdLog.Configure(1, "BankTransaction", 1000, 1, null, DateTime.Now, transactionDetails, transactionAudit);

        _transactionLogRepositoryMock.Setup(repo => repo.CreateTransactionLogAsync(It.IsAny<TransactionLog>()))
            .ReturnsAsync(createdLog);

        // Act
        var result =
            await _transactionLogService.CreateTransactionLogAsync(bankTransaction, transactionDetails,
                transactionAudit);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.BankTransactionId);
        Assert.Equal("BankTransaction", result.TransactionType);
        Assert.Equal(1000, result.Amount);
        Assert.Equal(1, result.AccountOriginId);
        Assert.Equal(transactionDetails, result.TransactionDetails);
        Assert.Equal(transactionAudit, result.TransactionAudit);
    }
}