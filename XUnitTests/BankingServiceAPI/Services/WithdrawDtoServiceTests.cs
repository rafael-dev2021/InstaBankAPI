using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using BankingServiceAPI.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace XUnitTests.BankingServiceAPI.Services;

public class WithdrawDtoServiceTests
{
    private readonly Mock<IWithdrawRepository> _withdrawRepositoryMock;
    private readonly Mock<IBankTransactionRepository> _bankTransactionRepositoryMock;
    private readonly Mock<ILogger<WithdrawDtoService>> _loggerMock;
    private readonly WithdrawDtoService _withdrawDtoService;

    public WithdrawDtoServiceTests()
    {
        _withdrawRepositoryMock = new Mock<IWithdrawRepository>();
        _bankTransactionRepositoryMock = new Mock<IBankTransactionRepository>();
        _loggerMock = new Mock<ILogger<WithdrawDtoService>>();
        _withdrawDtoService = new WithdrawDtoService(
            _withdrawRepositoryMock.Object,
            _bankTransactionRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task WithdrawAsync_ShouldWithdraw_WhenAccountIsValid()
    {
        // Arrange
        var user = new User();
        user.SetId("1");
        user.SetName("John");
        user.SetLastName("Doe");

        var account = new BankAccount();
        account.SetId(1);
        account.SetAccountNumber(123456);
        account.SetBalance(500m);
        account.SetUser(user);

        _withdrawRepositoryMock.Setup(r => r.GetByAccountNumberAsync(123456))
            .ReturnsAsync(account);

        // Act
        var result = await _withdrawDtoService.WithdrawAsync("1", 123456, 100m);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(account, result.AccountOrigin);
        Assert.Equal(100m, result.Amount);
        _bankTransactionRepositoryMock.Verify(r => r.CreateEntityAsync(It.IsAny<Withdraw>()), Times.Once);
    }

    [Fact]
    public async Task WithdrawAsync_ShouldThrowAccountNotFoundException_WhenAccountIsNull()
    {
        // Arrange
        _withdrawRepositoryMock.Setup(r => r.GetByAccountNumberAsync(It.IsAny<int>()))
            .ReturnsAsync((BankAccount)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AccountNotFoundException>(
            () => _withdrawDtoService.WithdrawAsync("1", 123456, 100m)
        );

        Assert.Equal("Account not found.", exception.Message);
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("Account number 123456 not found for user 1")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task WithdrawAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
    {
        // Arrange
        var user = new User();
        user.SetId("2");

        var account = new BankAccount();
        account.SetId(1);
        account.SetAccountNumber(123456);
        account.SetBalance(500m);
        account.SetUser(user);

        _withdrawRepositoryMock.Setup(r => r.GetByAccountNumberAsync(123456))
            .ReturnsAsync(account);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _withdrawDtoService.WithdrawAsync("1", 123456, 100m)
        );

        Assert.Equal("You are not authorized to perform this transaction.", exception.Message);
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("User 1 is not authorized to withdraw from account number 123456")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task WithdrawAsync_ShouldLogInfo_WhenAccountIsValid()
    {
        // Arrange
        var user = new User();
        user.SetId("1");
        user.SetName("John");
        user.SetLastName("Doe");

        var account = new BankAccount();
        account.SetId(1);
        account.SetAccountNumber(123456);
        account.SetBalance(500m);
        account.SetUser(user);

        _withdrawRepositoryMock.Setup(r => r.GetByAccountNumberAsync(123456))
            .ReturnsAsync(account);

        // Act
        var result = await _withdrawDtoService.WithdrawAsync("1", 123456, 100m);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(account, result.AccountOrigin);
        Assert.Equal(100m, result.Amount);
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("Attempting to withdraw 100 from account number 123456 for user 1")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!
            ),
            Times.Once
        );
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("Bank account found: John Doe, proceeding with withdrawal")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!
            ),
            Times.Once
        );
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("Withdraw entity created for account number 123456 with amount 100")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!
            ),
            Times.Once
        );
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains(
                        "Withdrawal of 100 from account number 123456 for user 1 successfully completed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!
            ),
            Times.Once
        );
    }
}