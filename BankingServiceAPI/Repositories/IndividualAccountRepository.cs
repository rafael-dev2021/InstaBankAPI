using BankingServiceAPI.Context;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingServiceAPI.Repositories;

public class IndividualAccountRepository(AppDbContext appDbContext) : IIndividualAccountRepository
{
    public async Task<IEnumerable<IndividualAccount>> GetEntitiesAsync()
    {
        return await appDbContext.IndividualAccounts
            .AsNoTracking()
            .Include(x => x.Address)
            .ToListAsync();
    }

    public async Task<IndividualAccount> GetByIdAsync(int? id)
    {
        return (await appDbContext.IndividualAccounts.FindAsync(id))!;
    }

    public async Task<IndividualAccount> CreateAsync(IndividualAccount entity)
    {
        await appDbContext.AddAsync(entity);
        await appDbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<IndividualAccount> UpdateAsync(IndividualAccount entity)
    {
        appDbContext.Update(entity);
        await appDbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<IndividualAccount> DeleteAsync(IndividualAccount entity)
    {
        appDbContext.Remove(entity);
        await appDbContext.SaveChangesAsync();
        return entity;
    }
}