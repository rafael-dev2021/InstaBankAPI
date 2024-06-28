using BankingServiceAPI.Models;

namespace BankingServiceAPI.Repositories.Interfaces;

public interface ITransferRepository : IAccountNumberRepository
{
    Task<BankAccount?> GetByCpfAsync(string cpf);
}