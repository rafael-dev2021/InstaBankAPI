using BankingServiceAPI.Models;

namespace XUnitTests.BankingServiceAPI.Models;

public class TransactionLogTests
{
    [Fact]
    public void TransactionLog_Initialization_SetsPropertiesCorrectly()
    {
        // Arrange
        const int id = 1;
        const int bankTransactionId = 2;
        var bankTransaction = new BankTransaction();
        const string transactionType = "Credit";
        const string description = "Test Description";
        const decimal amount = 100.50m;
        const int accountOriginId = 3;
        int? accountDestinationId = 4;
        var transactionDate = DateTime.Now;

        // Act
        var transactionLog = new TransactionLog
        {
            Id = id,
            BankTransactionId = bankTransactionId,
            BankTransaction = bankTransaction,
            TransactionType = transactionType,
            Description = description,
            Amount = amount,
            AccountOriginId = accountOriginId,
            AccountDestinationId = accountDestinationId,
            TransactionDate = transactionDate
        };

        // Assert
        Assert.Equal(id, transactionLog.Id);
        Assert.Equal(bankTransactionId, transactionLog.BankTransactionId);
        Assert.Equal(bankTransaction, transactionLog.BankTransaction);
        Assert.Equal(transactionType, transactionLog.TransactionType);
        Assert.Equal(description, transactionLog.Description);
        Assert.Equal(amount, transactionLog.Amount);
        Assert.Equal(accountOriginId, transactionLog.AccountOriginId);
        Assert.Equal(accountDestinationId, transactionLog.AccountDestinationId);
        Assert.Equal(transactionDate, transactionLog.TransactionDate);
    }

    [Fact]
    public void TransactionLog_DefaultTransactionDate_IsSetToNow()
    {
        // Act
        var transactionLog = new TransactionLog();

        // Assert
        Assert.Equal(DateTime.Now.Date, transactionLog.TransactionDate.Date);
    }
}