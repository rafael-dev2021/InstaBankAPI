using BankingServiceAPI.Exceptions;

namespace XUnitTests.BankingServiceAPI.Exceptions;

public class AccountNotFoundExceptionTests
{
    [Fact]
    public void AccountNotFoundException_Should_Set_Message()
    {
        // Arrange
        const string expectedMessage = "Account not found";

        // Act
        var exception = new AccountNotFoundException(expectedMessage);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void AccountNotFoundException_Should_Be_Of_Type_Exception()
    {
        // Act
        var exception = new AccountNotFoundException("Account not found");

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }
}