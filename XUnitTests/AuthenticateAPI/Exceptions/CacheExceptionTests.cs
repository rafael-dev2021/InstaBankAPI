using AuthenticateAPI.Exceptions;

namespace XUnitTests.AuthenticateAPI.Exceptions;

public class CacheExceptionTests
{
    [Fact]
    public void CacheException_Should_Set_Message_And_InnerException()
    {
        // Arrange
        const string expectedMessage = "Cache error";
        var innerException = new Exception("Inner exception message");

        // Act
        var exception = new CacheException(expectedMessage, innerException);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
    }

    [Fact]
    public void CacheException_Should_Be_Of_Type_Exception()
    {
        // Act
        var exception = new CacheException("Cache error", new Exception());

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }
}