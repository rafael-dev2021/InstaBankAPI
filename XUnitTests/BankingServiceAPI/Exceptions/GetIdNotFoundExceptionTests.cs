using BankingServiceAPI.Exceptions;

namespace XUnitTests.BankingServiceAPI.Exceptions;

public class GetIdNotFoundExceptionTests
{
    [Fact]
    public void GetIdNotFoundException_ShouldHaveCorrectMessage()
    {
        // Arrange
        const string expectedMessage = "ID not found.";

        // Act
        var exception = new GetIdNotFoundException(expectedMessage);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void GetIdNotFoundException_ShouldBeAssignableFromException()
    {
        // Arrange
        var exception = new GetIdNotFoundException("Test message");

        // Act & Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact]
    public void GetIdNotFoundException_ShouldNotHaveInnerException()
    {
        // Arrange
        var exception = new GetIdNotFoundException("Test message");

        // Act & Assert
        Assert.Null(exception.InnerException);
    }
}