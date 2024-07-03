using BankingServiceAPI.Models;

namespace XUnitTests.BankingServiceAPI.Models;

public class TransactionDetailsTests
{
    [Fact]
    public void TransactionDetails_Initialization_SetsPropertiesCorrectly()
    {
        // Arrange
        const string transactionStatus = "Completed";
        const string description = "Test Description";
        const string remarks = "Test Remarks";
        const string transactionReference = "Ref123";
        const string channel = "Online";
        const string errorDetails = "None";

        // Act
        var transactionDetails = new TransactionDetails();
        transactionDetails.Configure(
            transactionStatus,
            description,
            remarks,
            transactionReference,
            channel,
            errorDetails
        );

        // Assert
        Assert.Equal(transactionStatus, transactionDetails.TransactionStatus);
        Assert.Equal(description, transactionDetails.Description);
        Assert.Equal(remarks, transactionDetails.Remarks);
        Assert.Equal(transactionReference, transactionDetails.TransactionReference);
        Assert.Equal(channel, transactionDetails.Channel);
        Assert.Equal(errorDetails, transactionDetails.ErrorDetails);
    }

    [Fact]
    public void TransactionDetails_Setters_WorkCorrectly()
    {
        // Arrange
        const string transactionStatus = "Pending";
        const string description = "Another Description";
        const string remarks = "Additional Remarks";
        const string transactionReference = "Ref456";
        const string channel = "Mobile";
        const string errorDetails = "Error occurred";

        // Act
        var transactionDetails = new TransactionDetails();
        transactionDetails.SetTransactionStatus(transactionStatus);
        transactionDetails.SetDescription(description);
        transactionDetails.SetRemarks(remarks);
        transactionDetails.SetTransactionReference(transactionReference);
        transactionDetails.SetChannel(channel);
        transactionDetails.SetErrorDetails(errorDetails);

        // Assert
        Assert.Equal(transactionStatus, transactionDetails.TransactionStatus);
        Assert.Equal(description, transactionDetails.Description);
        Assert.Equal(remarks, transactionDetails.Remarks);
        Assert.Equal(transactionReference, transactionDetails.TransactionReference);
        Assert.Equal(channel, transactionDetails.Channel);
        Assert.Equal(errorDetails, transactionDetails.ErrorDetails);
    }
}