using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using FluentAssertions;

namespace XUnitTests.BankingServiceAPI.Models;

public class TransferTests
{
    [Fact]
    public void Execute_Should_Transfer_Amount_When_Origin_Has_Sufficient_Balance()
    {
        // Arrange
        var accountOrigin = new BankAccount();
        accountOrigin.SetBalance(200);
        var accountDestination = new BankAccount();
        accountDestination.SetBalance(100);
        var transfer = new Transfer();
        transfer.SetAccountOrigin(accountOrigin);
        transfer.SetAccountDestination(accountDestination);
        transfer.SetAmount(50);

        // Act
        transfer.Execute();

        // Assert
        accountOrigin.Balance.Should().Be(150);
        accountDestination.Balance.Should().Be(150);
    }

    [Fact]
    public void Execute_Should_Throw_InsufficientBalanceException_When_Origin_Has_Insufficient_Balance()
    {
        // Arrange
        var accountOrigin = new BankAccount();
        accountOrigin.SetBalance(40);
        var accountDestination = new BankAccount();
        accountDestination.SetBalance(100);
        
        var transfer = new Transfer();
        transfer.SetAccountOrigin(accountOrigin);
        transfer.SetAccountDestination(accountDestination);
        transfer.SetAmount(50);

        // Act
        var action = () => transfer.Execute();

        // Assert
        action.Should().Throw<BalanceInsufficientException>()
            .WithMessage("Insufficient balance in the origin account.");
    }
}