namespace AuthenticateAPI.Exceptions;

public class TokenRefreshException : Exception
{
    public TokenRefreshException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public TokenRefreshException(string message)
        : base(message)
    {
    }
}