using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using BankingServiceAPI.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace XUnitTests.BankingServiceAPI.Services;

public class DepositServiceTests
{
    private readonly Mock<IDepositRepository> _depositRepositoryMock;
    private readonly Mock<IBankTransactionRepository> _bankTransactionRepositoryMock;
    private readonly Mock<ILogger<DepositService>> _loggerMock;
    private readonly DepositService _depositService;

    public DepositServiceTests()
    {
        _depositRepositoryMock = new Mock<IDepositRepository>();
        _bankTransactionRepositoryMock = new Mock<IBankTransactionRepository>();
        _loggerMock = new Mock<ILogger<DepositService>>();
        _depositService = new DepositService(
            _depositRepositoryMock.Object,
            _bankTransactionRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task DepositAsync_ShouldExecuteDeposit_WhenAccountIsValid()
    {
        // Arrange
        var user = new User();
        user.SetId("1");
        user.SetEmail("user@example.com");

        var account = new BankAccount();
        account.SetAccountNumber(123456);
        account.SetUser(user);
        account.SetBalance(100m);

        _depositRepositoryMock.Setup(r => r.GetByAccountNumberAsync(123456))
            .ReturnsAsync(account);

        // Act
        var result = await _depositService.DepositAsync("1", 123456, 50m);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(account, result.AccountOrigin);
        Assert.Equal(account, result.AccountDestination);
        Assert.Equal(50m, result.Amount);

        _bankTransactionRepositoryMock.Verify(r => r.CreateEntityAsync(It.IsAny<Deposit>()), Times.Once);
        _loggerMock.Verify(l => l.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("Deposit of 50 to account number 123456 for user 1 successfully completed")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    [Fact]
    public async Task DepositAsync_ShouldThrowAccountNotFoundException_WhenAccountIsNull()
    {
        // Arrange
        _depositRepositoryMock.Setup(r => r.GetByAccountNumberAsync(123456))
            .ReturnsAsync((BankAccount)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AccountNotFoundException>(
            () => _depositService.DepositAsync("1", 123456, 50m)
        );

        Assert.Equal("Account not found.", exception.Message);
        _loggerMock.Verify(l => l.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("Account number 123456 not found for user 1")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    [Fact]
    public async Task DepositAsync_ShouldThrowUnauthorizedAccessException_WhenUserNotAuthorized()
    {
        // Arrange
        var user = new User();
        user.SetId("2");
        user.SetEmail("user@example.com");

        var account = new BankAccount();
        account.SetAccountNumber(123456);
        account.SetUser(user);
        account.SetBalance(100m);

        _depositRepositoryMock.Setup(r => r.GetByAccountNumberAsync(123456))
            .ReturnsAsync(account);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _depositService.DepositAsync("1", 123456, 50m)
        );

        Assert.Equal("User not authorized to deposit to this account.", exception.Message);
        _loggerMock.Verify(l => l.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("User 1 is not authorized to deposit to account number 123456")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}