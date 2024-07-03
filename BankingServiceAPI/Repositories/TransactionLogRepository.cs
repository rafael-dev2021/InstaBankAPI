using BankingServiceAPI.Context;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingServiceAPI.Repositories;

public class TransactionLogRepository(AppDbContext appDbContext) : ITransactionLogRepository
{
    public async Task<IEnumerable<TransactionLog>> GetAllTransactionLogsAsync()
    {
        return await appDbContext.TransactionLogs
            .AsNoTracking()
            .Include(x => x.BankTransaction)
            .ToListAsync();
    }

    public async Task<TransactionLog?> GetTransactionLogByIdAsync(int? id)
    {
        return await appDbContext.TransactionLogs
            .Include(x => x.BankTransaction)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<TransactionLog> CreateTransactionLogAsync(TransactionLog transactionLog)
    {
        await appDbContext.TransactionLogs.AddAsync(transactionLog);
        await appDbContext.SaveChangesAsync();
        return transactionLog;
    }

    public async Task<bool> DeleteTransactionLogAsync(int? id)
    {
        var transactionLog = await appDbContext.TransactionLogs.FindAsync(id);
        if (transactionLog == null)
            return false;

        appDbContext.TransactionLogs.Remove(transactionLog);
        await appDbContext.SaveChangesAsync();
        return true;
    }
}