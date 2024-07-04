using BankingServiceAPI.Models;

namespace XUnitTests.BankingServiceAPI.Models;

public class TransactionAuditTests
{
    [Fact]
    public void TransactionAudit_Initialization_SetsPropertiesCorrectly()
    {
        // Arrange
        const string initiatedBy = "UserA";
        const string approvedBy = "UserB";
        const string ipAddress = "192.168.0.1";
        const string deviceId = "Device123";
        const string location = "New York, USA";
        var timestamp = DateTime.Now;

        // Act
        var transactionAudit = new TransactionAudit();
        transactionAudit.Configure(
            initiatedBy,
            approvedBy,
            ipAddress,
            deviceId,
            location,
            timestamp
        );

        // Assert
        Assert.Equal(initiatedBy, transactionAudit.InitiatedBy);
        Assert.Equal(approvedBy, transactionAudit.ApprovedBy);
        Assert.Equal(ipAddress, transactionAudit.IpAddress);
        Assert.Equal(deviceId, transactionAudit.DeviceId);
        Assert.Equal(location, transactionAudit.Location);
        Assert.Equal(timestamp, transactionAudit.Timestamp);
    }

    [Fact]
    public void TransactionAudit_Setters_WorkCorrectly()
    {
        // Arrange
        const string initiatedBy = "UserC";
        const string approvedBy = "UserD";
        const string ipAddress = "192.168.1.1";
        const string deviceId = "Device456";
        const string location = "San Francisco, USA";
        var timestamp = DateTime.Now;

        // Act
        var transactionAudit = new TransactionAudit();
        transactionAudit.SetInitiatedBy(initiatedBy);
        transactionAudit.SetApprovedBy(approvedBy);
        transactionAudit.SetIpAddress(ipAddress);
        transactionAudit.SetDeviceId(deviceId);
        transactionAudit.SetLocation(location);
        transactionAudit.SetTimestamp(timestamp);

        // Assert
        Assert.Equal(initiatedBy, transactionAudit.InitiatedBy);
        Assert.Equal(approvedBy, transactionAudit.ApprovedBy);
        Assert.Equal(ipAddress, transactionAudit.IpAddress);
        Assert.Equal(deviceId, transactionAudit.DeviceId);
        Assert.Equal(location, transactionAudit.Location);
        Assert.Equal(timestamp, transactionAudit.Timestamp);
    }
}