using AutoMapper;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using BankingServiceAPI.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace XUnitTests.BankingServiceAPI.Services;

public class TransferDtoServiceTests
{
    private readonly Mock<ITransferRepository> _transferRepositoryMock;
    private readonly Mock<IBankTransactionRepository> _bankTransactionRepositoryMock;
    private readonly TransferDtoService _transferDtoService;
    private readonly Mock<IMapper> _mockMapper;

    public TransferDtoServiceTests()
    {
        _transferRepositoryMock = new Mock<ITransferRepository>();
        _bankTransactionRepositoryMock = new Mock<IBankTransactionRepository>();
        _mockMapper = new Mock<IMapper>();
        Mock<ILogger<TransferDtoService>> loggerMock = new();
        _transferDtoService = new TransferDtoService(
            _transferRepositoryMock.Object,
            _bankTransactionRepositoryMock.Object,
            loggerMock.Object,
            _mockMapper.Object
        );
    }

     [Fact]
    public async Task TransferAsync_ShouldExecuteTransfer_WhenAccountsAreValid()
    {
        // Arrange
        var originUser = new User();
        originUser.SetId("1");
        originUser.SetCpf("123.456.789-00");

        var originAccount = new BankAccount();
        originAccount.SetAccountNumber(123456);
        originAccount.SetBalance(200m);
        originAccount.SetUser(originUser);

        var destinationAccount = new BankAccount();
        destinationAccount.SetAccountNumber(654321);
        destinationAccount.SetBalance(50m);

        _transferRepositoryMock.Setup(r => r.GetByAccountNumberAsync(123456))
            .ReturnsAsync(originAccount);
        _transferRepositoryMock.Setup(r => r.GetByAccountNumberAsync(654321))
            .ReturnsAsync(destinationAccount);


        var transfer = new Transfer();
        transfer.SetAccountOrigin(originAccount);
        transfer.SetAccountOriginId(originAccount.Id);
        transfer.SetAccountDestination(destinationAccount);
        transfer.SetAccountDestinationId(destinationAccount.Id);
        transfer.SetAmount(100m);
        transfer.SetTransferDate(DateTime.Now);

        _mockMapper.Setup(m => m.Map<TransferByBankAccountNumberDtoResponse>(It.IsAny<Transfer>()))
            .Returns(new TransferByBankAccountNumberDtoResponse(
                transfer.Id,
                transfer.AccountOrigin!.User!.Name!,
                transfer.AccountOrigin!.User!.LastName!,
                transfer.AccountDestination!.AccountNumber,
                transfer.Amount,
                DateTime.Now));

        // Act
        var result = await _transferDtoService.TransferByBankAccountNumberDtoAsync("1", 123456, 654321, 100m);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transfer.Id, result.TransactionId);
        Assert.Equal(originUser.Name, result.Name);
        Assert.Equal(originUser.LastName, result.LastName);
        Assert.Equal(654321, result.DestinationBankAccountNumber);
        Assert.Equal(100m, result.Amount);

        _bankTransactionRepositoryMock.Verify(r => r.CreateEntityAsync(It.IsAny<Transfer>()), Times.Once);
    }

    [Fact]
    public async Task TransferAsync_ShouldThrowAccountNotFoundException_WhenOriginAccountIsNull()
    {
        // Arrange
        _transferRepositoryMock.Setup(r => r.GetByAccountNumberAsync(123456))
            .ReturnsAsync((BankAccount)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AccountNotFoundException>(
            () => _transferDtoService.TransferByBankAccountNumberDtoAsync("1", 123456, 654321, 100m)
        );

        Assert.Equal("Origin account not found.", exception.Message);
    }

    [Fact]
    public async Task TransferAsync_ShouldThrowAccountNotFoundException_WhenDestinationAccountIsNull()
    {
        // Arrange
        var originAccount = new BankAccount();
        originAccount.SetAccountNumber(123456);

        _transferRepositoryMock.Setup(r => r.GetByAccountNumberAsync(123456))
            .ReturnsAsync(originAccount);
        _transferRepositoryMock.Setup(r => r.GetByAccountNumberAsync(654321))
            .ReturnsAsync((BankAccount)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AccountNotFoundException>(
            () => _transferDtoService.TransferByBankAccountNumberDtoAsync("1", 123456, 654321, 100m)
        );

        Assert.Equal("Destination account not found.", exception.Message);
    }

    [Fact]
    public async Task TransferByCpfAsync_ShouldExecuteTransfer_WhenAccountsAreValid()
    {
        // Arrange
        var originUser = new User();
        originUser.SetId("1");
        originUser.SetCpf("123.456.789-00");

        var originAccount = new BankAccount();
        originAccount.SetUser(originUser);
        originAccount.SetBalance(300m);

        var destinationUser = new User();
        destinationUser.SetCpf("987.654.321-00");

        var destinationAccount = new BankAccount();
        destinationAccount.SetUser(destinationUser);

        _transferRepositoryMock.Setup(r => r.GetByCpfAsync("123.456.789-00"))
            .ReturnsAsync(originAccount);
        _transferRepositoryMock.Setup(r => r.GetByCpfAsync("987.654.321-00"))
            .ReturnsAsync(destinationAccount);

        var transfer = new Transfer();
        transfer.SetAccountOrigin(originAccount);
        transfer.SetAccountOriginId(originAccount.Id);
        transfer.SetAccountDestination(destinationAccount);
        transfer.SetAccountDestinationId(destinationAccount.Id);
        transfer.SetAmount(300m);
        transfer.SetTransferDate(DateTime.Now);

        _mockMapper.Setup(m => m.Map<TransferByCpfDtoResponse>(It.IsAny<Transfer>()))
            .Returns(new TransferByCpfDtoResponse(
                transfer.Id,
                transfer.AccountOrigin!.User!.Name!,
                transfer.AccountOrigin!.User!.LastName!,
                transfer.AccountOrigin!.User!.Cpf!,
                transfer.Amount,
                DateTime.Now));

        // Act
        var result = await _transferDtoService.TransferByCpfDtoAsync("1", "123.456.789-00", "987.654.321-00", 200m);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transfer.Id, result.TransactionId);
        Assert.Equal(originUser.Name, result.Name);
        Assert.Equal(originUser.LastName, result.LastName);
        Assert.Equal(originUser.Cpf, result.DestinationCpf);
        Assert.Equal(300m, result.Amount);

        _bankTransactionRepositoryMock.Verify(r => r.CreateEntityAsync(It.IsAny<Transfer>()), Times.Once);
    }

    [Fact]
    public async Task TransferByCpfAsync_ShouldThrowAccountNotFoundException_WhenOriginAccountIsNull()
    {
        // Arrange
        _transferRepositoryMock.Setup(r => r.GetByCpfAsync("123.456.789-00"))
            .ReturnsAsync((BankAccount)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AccountNotFoundException>(
            () => _transferDtoService.TransferByCpfDtoAsync("1", "123.456.789-00", "987.654.321-00", 200m)
        );

        Assert.Equal("Origin account not found.", exception.Message);
    }

    [Fact]
    public async Task TransferByCpfAsync_ShouldThrowAccountNotFoundException_WhenDestinationAccountIsNull()
    {
        // Arrange
        var originUser = new User();
        originUser.SetCpf("123.456.789-00");

        var originAccount = new BankAccount();
        originAccount.SetUser(originUser);
        originAccount.SetBalance(300m);

        _transferRepositoryMock.Setup(r => r.GetByCpfAsync("123.456.789-00"))
            .ReturnsAsync(originAccount);
        _transferRepositoryMock.Setup(r => r.GetByCpfAsync("987.654.321-00"))
            .ReturnsAsync((BankAccount)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AccountNotFoundException>(
            () => _transferDtoService.TransferByCpfDtoAsync("1", "123.456.789-00", "987.654.321-00", 200m)
        );

        Assert.Equal("Destination account not found.", exception.Message);
    }

    [Fact]
    public async Task TransferAsync_ShouldThrowUnauthorizedAccessException_WhenUserNotAuthorized()
    {
        // Arrange
        var originUser = new User();
        originUser.SetId("2");
        originUser.SetCpf("123.456.789-00");

        var originAccount = new BankAccount();
        originAccount.SetAccountNumber(123456);
        originAccount.SetBalance(200m);
        originAccount.SetUser(originUser);

        var destinationAccount = new BankAccount();
        destinationAccount.SetAccountNumber(654321);
        destinationAccount.SetBalance(50m);

        _transferRepositoryMock.Setup(r => r.GetByAccountNumberAsync(123456))
            .ReturnsAsync(originAccount);
        _transferRepositoryMock.Setup(r => r.GetByAccountNumberAsync(654321))
            .ReturnsAsync(destinationAccount);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _transferDtoService.TransferByBankAccountNumberDtoAsync("1", 123456, 654321, 100m)
        );

        Assert.Equal("User not authorized to transfer from this account.", exception.Message);
    }

    [Fact]
    public async Task TransferByCpfAsync_ShouldThrowUnauthorizedAccessException_WhenUserNotAuthorized()
    {
        // Arrange
        var originUser = new User();
        originUser.SetId("2");
        originUser.SetCpf("123.456.789-00");

        var originAccount = new BankAccount();
        originAccount.SetUser(originUser);
        originAccount.SetBalance(300m);

        var destinationUser = new User();
        destinationUser.SetCpf("987.654.321-00");

        var destinationAccount = new BankAccount();
        destinationAccount.SetUser(destinationUser);

        _transferRepositoryMock.Setup(r => r.GetByCpfAsync("123.456.789-00"))
            .ReturnsAsync(originAccount);
        _transferRepositoryMock.Setup(r => r.GetByCpfAsync("987.654.321-00"))
            .ReturnsAsync(destinationAccount);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _transferDtoService.TransferByCpfDtoAsync("1", "123.456.789-00", "987.654.321-00", 200m)
        );

        Assert.Equal("User not authorized to transfer from this account.", exception.Message);
    }
}