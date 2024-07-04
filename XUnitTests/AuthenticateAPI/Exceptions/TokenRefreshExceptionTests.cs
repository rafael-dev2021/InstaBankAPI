using AuthenticateAPI.Exceptions;

namespace XUnitTests.AuthenticateAPI.Exceptions;

public class TokenRefreshExceptionTests
{
    [Fact]
    public void TokenRefreshException_Should_Set_Message()
    {
        // Arrange
        const string expectedMessage = "Token refresh error";

        // Act
        var exception = new TokenRefreshException(expectedMessage);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void TokenRefreshException_Should_Set_Message_And_InnerException()
    {
        // Arrange
        const string expectedMessage = "Token refresh error";
        var innerException = new Exception("Inner exception message");

        // Act
        var exception = new TokenRefreshException(expectedMessage, innerException);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
    }

    [Fact]
    public void TokenRefreshException_Should_Be_Of_Type_Exception()
    {
        // Act
        var exception = new TokenRefreshException("Token refresh error");

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }
}