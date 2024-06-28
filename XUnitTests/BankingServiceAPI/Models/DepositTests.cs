using BankingServiceAPI.Models;
using FluentAssertions;

namespace XUnitTests.BankingServiceAPI.Models;

public class DepositTests
{
    [Fact]
    public void Execute_Should_Add_Amount_To_Account_When_AccountOrigin_Is_Set()
    {
        // Arrange
        var accountOrigin = new BankAccount();
        accountOrigin.SetBalance(100);
        
        var deposit = new Deposit();
        deposit.SetAccountOrigin(accountOrigin);
        deposit.SetAmount(50);

        // Act
        deposit.Execute();

        // Assert
        accountOrigin.Balance.Should().Be(150);
    }

    [Fact]
    public void Execute_Should_Throw_InvalidOperationException_When_AccountOrigin_Is_Null()
    {
        // Arrange
        var deposit = new Deposit();
        deposit.SetAmount(50);

        // Act
        var action = () => deposit.Execute();

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Origin account is required for deposit.");
    }

    [Fact]
    public void Execute_Should_Handle_Negative_Amount_Correctly()
    {
        // Arrange
        var accountOrigin = new BankAccount();
        accountOrigin.SetBalance(100);
        
        var deposit = new Deposit();
        deposit.SetAccountOrigin(accountOrigin);
        deposit.SetAmount(-50);

        // Act
        deposit.Execute();

        // Assert
        accountOrigin.Balance.Should().Be(50);
    }
}