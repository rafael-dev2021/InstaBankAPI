namespace BankingServiceAPI.Exceptions;

public class AccountNotFoundException(string message) : Exception(message);