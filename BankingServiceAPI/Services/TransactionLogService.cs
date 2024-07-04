using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using BankingServiceAPI.Services.Interfaces;
using Serilog;

namespace BankingServiceAPI.Services;

public class TransactionLogService(
    ITransactionLogRepository transactionLogRepository,
    IHttpContextAccessor httpContextAccessor) : ITransactionLogService
{
    public async Task<TransactionDetails> CreateTransactionDetailsAsync<T>(T transaction) where T : BankTransaction
    {
        Log.Information(
            "[TRANSACTION_DETAILS] Creating TransactionDetails for [{TransactionType}] with Id: [{TransactionId}]",
            typeof(T).Name, transaction.Id);

        var transactionDetails = new TransactionDetails();
        transactionDetails.Configure(
            "Completed",
            $"{typeof(T).Name} operation",
            "Successfully completed",
            Guid.NewGuid().ToString(),
            "Internet Banking",
            null);

        Log.Information(
            "[TRANSACTION_DETAILS] TransactionDetails created for [{TransactionType}] with Id: [{TransactionId}]",
            typeof(T).Name, transaction.Id);
        return await Task.FromResult(transactionDetails);
    }

    public async Task<TransactionAudit> CreateTransactionAuditAsync<T>(T transaction) where T : BankTransaction
    {
        Log.Information(
            "[TRANSACTION_AUDIT] Creating TransactionAudit for [{TransactionType}] with Id: [{TransactionId}]",
            typeof(T).Name, transaction.Id);

        var ipAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
        var deviceId = GetDeviceId(httpContextAccessor.HttpContext!);

        var transactionAudit = new TransactionAudit();
        transactionAudit.Configure(
            transaction.AccountOrigin!.User!.Id,
            "System",
            ipAddress,
            deviceId,
            "Brazil",
            DateTime.Now);

        Log.Information(
            "[TRANSACTION_AUDIT] TransactionAudit created for [{TransactionType}] with Id: [{TransactionId}]",
            typeof(T).Name, transaction.Id);
        return await Task.FromResult(transactionAudit);
    }

    public async Task<TransactionLog> CreateTransactionLogAsync<T>(T transaction, TransactionDetails transactionDetails,
        TransactionAudit transactionAudit) where T : BankTransaction
    {
        Log.Information("[TRANSACTION_LOG] Creating TransactionLog for [{TransactionType}]  with Id: [{TransactionId}]",
            typeof(T).Name, transaction.Id);

        var transactionLog = new TransactionLog();
        transactionLog.Configure(
            transaction.Id,
            typeof(T).Name,
            transaction.Amount,
            transaction.AccountOriginId,
            transaction.AccountDestinationId,
            DateTime.Now,
            transactionDetails,
            transactionAudit
        );

        var createdLog = await transactionLogRepository.CreateTransactionLogAsync(transactionLog);
        Log.Information("[TRANSACTION_LOG] TransactionLog created for [{TransactionType}] with Id: [{TransactionId}]",
            typeof(T).Name, transaction.Id);
        return createdLog;
    }

    private static string GetDeviceId(HttpContext context)
    {
        var deviceId = context.Request.Cookies["DeviceId"];
        if (!string.IsNullOrEmpty(deviceId)) return deviceId;

        deviceId = Guid.NewGuid().ToString();
        context.Response.Cookies.Append("DeviceId", deviceId,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict 
            });
        return deviceId;
    }
}