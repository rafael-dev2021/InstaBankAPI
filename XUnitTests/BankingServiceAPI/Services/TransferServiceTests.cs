using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using BankingServiceAPI.Services;
using Moq;

namespace XUnitTests.BankingServiceAPI.Services;

public class TransferServiceTests
{
    private readonly Mock<ITransferRepository> _transferRepositoryMock;
    private readonly Mock<IBankTransactionRepository> _bankTransactionRepositoryMock;
    private readonly TransferService _transferService;

    public TransferServiceTests()
    {
        _transferRepositoryMock = new Mock<ITransferRepository>();
        _bankTransactionRepositoryMock = new Mock<IBankTransactionRepository>();
        _transferService = new TransferService(
            _transferRepositoryMock.Object,
            _bankTransactionRepositoryMock.Object
        );
    }

    [Fact]
    public async Task TransferAsync_ShouldExecuteTransfer_WhenAccountsAreValid()
    {
        // Arrange
        var originAccount = new BankAccount();
        originAccount.SetAccountNumber(123456);
        originAccount.SetBalance(200m);

        var destinationAccount = new BankAccount();
        destinationAccount.SetAccountNumber(654321);
        destinationAccount.SetBalance(50m);

        _transferRepositoryMock.Setup(r => r.GetByAccountNumberAsync(123456))
            .ReturnsAsync(originAccount);
        _transferRepositoryMock.Setup(r => r.GetByAccountNumberAsync(654321))
            .ReturnsAsync(destinationAccount);

        // Act
        var result = await _transferService.TransferAsync(123456, 654321, 100m);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(originAccount, result.AccountOrigin);
        Assert.Equal(destinationAccount, result.AccountDestination);
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
            () => _transferService.TransferAsync(123456, 654321, 100m)
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
            () => _transferService.TransferAsync(123456, 654321, 100m)
        );

        Assert.Equal("Destination account not found.", exception.Message);
    }

    [Fact]
    public async Task TransferByCpfAsync_ShouldExecuteTransfer_WhenAccountsAreValid()
    {
        // Arrange
        var originUser = new User();
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

        // Act
        var result = await _transferService.TransferByCpfAsync("123.456.789-00", "987.654.321-00", 200m);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(originAccount, result.AccountOrigin);
        Assert.Equal(destinationAccount, result.AccountDestination);
        Assert.Equal(200m, result.Amount);

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
            () => _transferService.TransferByCpfAsync("123.456.789-00", "987.654.321-00", 200m)
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
            () => _transferService.TransferByCpfAsync("123.456.789-00", "987.654.321-00", 200m)
        );

        Assert.Equal("Destination account not found.", exception.Message);
    }
}