namespace BankingServiceAPI.Models;

public class TransactionDetails
{
    public string? TransactionStatus { get; private set; }
    public string? Description { get; private set; }
    public string? Remarks { get; private set; }
    public string? TransactionReference { get; private set; }
    public string? Channel { get; private set; }
    public string? ErrorDetails { get; private set; }

    public void SetTransactionStatus(string? transactionStatus) => TransactionStatus = transactionStatus;
    public void SetDescription(string? description) => Description = description;
    public void SetRemarks(string? remarks) => Remarks = remarks;
    public void SetTransactionReference(string? transactionReference) => TransactionReference = transactionReference;
    public void SetChannel(string? channel) => Channel = channel;
    public void SetErrorDetails(string? errorDetails) => ErrorDetails = errorDetails;

    public void Configure(string? transactionStatus, string? description, string? remarks, string? transactionReference,
        string? channel, string? errorDetails)
    {
        SetTransactionStatus(transactionStatus);
        SetDescription(description);
        SetRemarks(remarks);
        SetTransactionReference(transactionReference);
        SetChannel(channel);
        SetErrorDetails(errorDetails);
    }
}