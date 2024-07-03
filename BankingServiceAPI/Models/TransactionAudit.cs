namespace BankingServiceAPI.Models;

public class TransactionAudit
{
    public string? InitiatedBy { get; private set; }
    public string? ApprovedBy { get; private set; }
    public string? IpAddress { get; private set; }
    public string? DeviceId { get; private set; }
    public string? Location { get; private set; }
    public DateTime Timestamp { get; private set; }

    public void SetInitiatedBy(string? initiatedBy) => InitiatedBy = initiatedBy;
    public void SetApprovedBy(string? approvedBy) => ApprovedBy = approvedBy;
    public void SetIpAddress(string? ipAddress) => IpAddress = ipAddress;
    public void SetDeviceId(string? deviceId) => DeviceId = deviceId;
    public void SetLocation(string? location) => Location = location;
    public void SetTimestamp(DateTime timestamp) => Timestamp = timestamp;

    public void Configure(string? initiatedBy, string? approvedBy, string? ipAddress, string? deviceId,
        string? location, DateTime timestamp)
    {
        SetInitiatedBy(initiatedBy);
        SetApprovedBy(approvedBy);
        SetIpAddress(ipAddress);
        SetDeviceId(deviceId);
        SetLocation(location);
        SetTimestamp(timestamp);
    }
}