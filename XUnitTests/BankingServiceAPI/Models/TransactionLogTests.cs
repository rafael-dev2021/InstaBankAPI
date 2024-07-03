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
        const decimal amount = 100.50m;
        const int accountOriginId = 3;
        int? accountDestinationId = 4;
        var transactionDate = DateTime.Now;
        var transactionDetails = new TransactionDetails();
        var transactionAudit = new TransactionAudit();

        // Act
        var transactionLog = new TransactionLog
        {
            Id = id
        };
        transactionLog.SetBankTransaction(bankTransaction);
        transactionLog.Configure(
            bankTransactionId,
            transactionType,
            amount,
            accountOriginId,
            accountDestinationId,
            transactionDate,
            transactionDetails,
            transactionAudit
        );

        // Assert
        Assert.Equal(id, transactionLog.Id);
        Assert.Equal(bankTransactionId, transactionLog.BankTransactionId);
        Assert.Equal(bankTransaction, transactionLog.BankTransaction);
        Assert.Equal(transactionType, transactionLog.TransactionType);
        Assert.Equal(amount, transactionLog.Amount);
        Assert.Equal(accountOriginId, transactionLog.AccountOriginId);
        Assert.Equal(accountDestinationId, transactionLog.AccountDestinationId);
        Assert.Equal(transactionDate, transactionLog.TransactionDate);
        Assert.Equal(transactionDetails, transactionLog.TransactionDetails);
        Assert.Equal(transactionAudit, transactionLog.TransactionAudit);
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