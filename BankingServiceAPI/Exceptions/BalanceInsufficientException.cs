namespace BankingServiceAPI.Exceptions;

public class BalanceInsufficientException(string message) : Exception(message);