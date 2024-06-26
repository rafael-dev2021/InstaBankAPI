using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using FluentAssertions;

namespace XUnitTests.BankingServiceAPI.Models;

public class WithdrawTests
{
    [Fact]
    public void Execute_Should_Deduct_Amount_From_Account_When_Balance_Is_Sufficient()
    {
        // Arrange
        var accountOrigin = new BankAccount();
        accountOrigin.SetBalance(200);

        var withdraw = new Withdraw();
        withdraw.SetAccountOrigin(accountOrigin);
        withdraw.SetAmount(50);

        // Act
        withdraw.Execute();

        // Assert
        accountOrigin.Balance.Should().Be(150);
    }

    [Fact]
    public void Execute_Should_Throw_BalanceInsufficientException_When_Balance_Is_Insufficient()
    {
        // Arrange
        var accountOrigin = new BankAccount();
        accountOrigin.SetBalance(40);

        var withdraw = new Withdraw();
        withdraw.SetAccountOrigin(accountOrigin);
        withdraw.SetAmount(50);


        // Act
        var action = () => withdraw.Execute();

        // Assert
        action.Should().Throw<BalanceInsufficientException>()
            .WithMessage("Insufficient balance in the origin account.");
    }

    [Fact]
    public void Execute_Should_Handle_Exact_Balance_Correctly()
    {
        // Arrange
        var accountOrigin = new BankAccount();
        accountOrigin.SetBalance(50);

        var withdraw = new Withdraw();
        withdraw.SetAccountOrigin(accountOrigin);
        withdraw.SetAmount(50);


        // Act
        withdraw.Execute();

        // Assert
        accountOrigin.Balance.Should().Be(0);
    }
}