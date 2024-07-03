namespace AuthenticateAPI.Exceptions;

public class TokenRefreshException(string message, Exception innerException) : Exception(message, innerException);
