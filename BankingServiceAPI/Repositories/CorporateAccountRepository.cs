using BankingServiceAPI.Context;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingServiceAPI.Repositories;

public class CorporateAccountRepository(AppDbContext appDbContext) : ICorporateAccountRepository
{
    public async Task<IEnumerable<CorporateAccount>> GetEntitiesAsync()
    {
        return await appDbContext.CorporateAccounts
            .AsNoTracking()
            .Include(x => x.Address)
            .ToListAsync();
    }

    public async Task<CorporateAccount> GetByIdAsync(int? id)
    {
        return (await appDbContext.CorporateAccounts.FindAsync(id))!;
    }

    public async Task<CorporateAccount> CreateAsync(CorporateAccount entity)
    {
        await appDbContext.AddAsync(entity);
        await appDbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<CorporateAccount> UpdateAsync(CorporateAccount entity)
    {
        appDbContext.Update(entity);
        await appDbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<CorporateAccount> DeleteAsync(CorporateAccount entity)
    {
        appDbContext.Remove(entity);
        await appDbContext.SaveChangesAsync();
        return entity;
    }
}