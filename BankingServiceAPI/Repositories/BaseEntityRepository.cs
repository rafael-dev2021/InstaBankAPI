using BankingServiceAPI.Context;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingServiceAPI.Repositories;

public class BaseEntityRepository(AppDbContext appDbContext) : IBaseEntityRepository
{
    public async Task<IEnumerable<BaseEntity>> ListAccountsAsync()
    {
        return await appDbContext.BaseEntities
            .AsNoTracking()
            .Include(x => x.Address)
            .ToListAsync();
    }

    public async Task<BaseEntity> GetAccountByIdAsync(Guid id)
    {
        return (await appDbContext.BaseEntities
            .Include(x => x.Address)
            .FirstOrDefaultAsync(x => x.Id == id))!;
    }
}