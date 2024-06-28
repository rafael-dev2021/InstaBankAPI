using BankingServiceAPI.Exceptions;

namespace XUnitTests.BankingServiceAPI.Exceptions;

public class BankAccountDtoServiceExceptionTests
{
    [Fact]
    public void BankAccountDtoServiceException_ShouldHaveCorrectMessage()
    {
        // Arrange
        const string expectedMessage = "An error occurred.";
        var innerException = new Exception("Inner exception message");

        // Act
        var exception = new BankAccountDtoServiceException(expectedMessage, innerException);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
    }

    [Fact]
    public void BankAccountDtoServiceException_ShouldBeAssignableFromException()
    {
        // Arrange
        var exception = new BankAccountDtoServiceException("Test message", new Exception());

        // Act & Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact]
    public void BankAccountDtoServiceException_ShouldContainInnerException()
    {
        // Arrange
        var innerException = new Exception("Inner exception message");

        // Act
        var exception = new BankAccountDtoServiceException("Test message", innerException);

        // Assert
        Assert.NotNull(exception.InnerException);
        Assert.Equal(innerException, exception.InnerException);
    }
}