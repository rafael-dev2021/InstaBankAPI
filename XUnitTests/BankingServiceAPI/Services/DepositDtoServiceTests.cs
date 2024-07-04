using AutoMapper;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using BankingServiceAPI.Services;
using BankingServiceAPI.Services.Interfaces;
using Moq;

namespace XUnitTests.BankingServiceAPI.Services;

public class DepositDtoServiceTests
{
    private readonly Mock<IDepositRepository> _depositRepositoryMock;
    private readonly Mock<IBankTransactionRepository> _bankTransactionRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly DepositDtoService _depositDtoService;

    public DepositDtoServiceTests()
    {
        _depositRepositoryMock = new Mock<IDepositRepository>();
        _bankTransactionRepositoryMock = new Mock<IBankTransactionRepository>();
        _mapperMock = new Mock<IMapper>();
        Mock<ITransactionLogService> transactionLogServiceMock = new();

        _depositDtoService = new DepositDtoService(
            _depositRepositoryMock.Object,
            _bankTransactionRepositoryMock.Object,
            transactionLogServiceMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task DepositAsync_ShouldExecuteDeposit_WhenAccountIsValid()
    {
        // Arrange
        var user = new User();
        user.SetId("1");
        user.SetEmail("user@example.com");

        var account = new BankAccount();
        account.SetId(1);
        account.SetAccountNumber(123456);
        account.SetUser(user);
        account.SetBalance(100m);

        _depositRepositoryMock.Setup(r => r.GetByAccountNumberAsync(123456))
            .ReturnsAsync(account);

        var deposit = new Deposit();
        deposit.SetAccountOrigin(account);
        deposit.SetAccountOriginId(account.Id);
        deposit.SetAccountDestination(account);
        deposit.SetAccountDestinationId(account.Id);
        deposit.SetAmount(50m);
        deposit.SetTransferDate(DateTime.Now);

        _mapperMock.Setup(m => m.Map<DepositDtoResponse>(It.IsAny<Deposit>()))
            .Returns(new DepositDtoResponse(
                deposit.Id,
                deposit.AccountOrigin!.User!.Name!,
                deposit.AccountOrigin!.User!.LastName!,
                deposit.AccountOrigin!.User!.Cpf!,
                deposit.AccountOrigin.AccountNumber,
                deposit.Amount,
                DateTime.Now));

        // Act
        var result = await _depositDtoService.DepositDtoAsync("1", 123456, 50m);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(123456, result.BankAccountNumber);
        Assert.Equal(50m, result.Amount);

        _bankTransactionRepositoryMock.Verify(r => r.CreateEntityAsync(It.IsAny<Deposit>()), Times.Once);
    }

    [Fact]
    public async Task DepositAsync_ShouldThrowAccountNotFoundException_WhenAccountIsNull()
    {
        // Arrange
        _depositRepositoryMock.Setup(r => r.GetByAccountNumberAsync(123456))
            .ReturnsAsync((BankAccount)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AccountNotFoundException>(
            () => _depositDtoService.DepositDtoAsync("1", 123456, 50m)
        );

        Assert.Equal("Account not found.", exception.Message);
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
            () => _depositDtoService.DepositDtoAsync("1", 123456, 50m)
        );

        Assert.Equal("User not authorized to deposit to this account.", exception.Message);
    }
}