namespace BankingServiceAPI.Exceptions;

public class BankAccountDtoServiceException(string message, Exception innerException)
    : Exception(message, innerException);