using BankingServiceAPI.Models;

namespace BankingServiceAPI.Services.Interfaces;

public interface ITransactionLogService
{
    Task<TransactionDetails> CreateTransactionDetailsAsync<T>(T transaction) where T : BankTransaction;
    Task<TransactionAudit> CreateTransactionAuditAsync<T>(T transaction) where T : BankTransaction;
    Task<TransactionLog> CreateTransactionLogAsync<T>(T transaction, TransactionDetails transactionDetails, TransactionAudit transactionAudit) where T : BankTransaction;
}