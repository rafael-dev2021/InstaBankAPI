using BankingServiceAPI.Models;

namespace BankingServiceAPI.Repositories.Interfaces;

public interface ITransferRepository
{
    Task<BankAccount?> GetByAccountNumberAsync(int accountNumber);
    Task<BankAccount?> GetByCpfAsync(string cpf);
}