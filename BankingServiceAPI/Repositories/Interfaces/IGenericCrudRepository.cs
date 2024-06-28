namespace BankingServiceAPI.Repositories.Interfaces;

public interface IGenericCrudRepository<T> where T : class
{
    Task<IEnumerable<T>> GetEntitiesAsync();
    Task<T?> GetEntityByIdAsync(int? id);
    Task<T> CreateEntityAsync(T entity);
    Task<bool> DeleteEntityAsync(int? id);
}