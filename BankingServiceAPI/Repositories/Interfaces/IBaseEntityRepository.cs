using BankingServiceAPI.Models;

namespace BankingServiceAPI.Repositories.Interfaces;

public interface IBaseEntityRepository
{
    Task<IEnumerable<BaseEntity>> ListAccountsAsync();
    Task<BaseEntity> GetAccountByIdAsync(Guid id);
}