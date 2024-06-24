using BankingServiceAPI.Exceptions;

namespace XUnitTests.BankingServiceAPI.Exceptions;

public class BalanceInsufficientExceptionTests
{
    [Fact]
    public void BalanceInsufficientException_ShouldHaveCorrectMessage()
    {
        // Arrange
        const string expectedMessage = "Insufficient balance for this operation.";

        // Act
        var exception = new BalanceInsufficientException(expectedMessage);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void BalanceInsufficientException_ShouldBeAssignableFromException()
    {
        // Act
        var exception = new BalanceInsufficientException("Test message");

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }
}