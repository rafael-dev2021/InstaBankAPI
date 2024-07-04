using BankingServiceAPI.Models;

namespace BankingServiceAPI.Repositories.Interfaces
{
    public interface ITransactionLogRepository
    {
        Task<IEnumerable<TransactionLog>> GetAllTransactionLogsAsync();
        Task<TransactionLog?> GetTransactionLogByIdAsync(int? id);
        Task<TransactionLog> CreateTransactionLogAsync(TransactionLog transactionLog);
        Task<bool> DeleteTransactionLogAsync(int? id);
    }
}