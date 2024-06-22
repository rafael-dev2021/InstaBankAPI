using BankingServiceAPI.Context;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingServiceAPI.Repositories;

public class AddressRepository(AppDbContext appDbContext) : IAddressRepository
{
    public async Task<IEnumerable<Address>> GetEntitiesAsync()
    {
        return await appDbContext.Addresses
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Address> GetByIdAsync(int? id)
    {
        return (await appDbContext.Addresses.FindAsync(id))!;
    }

    public async Task<Address> CreateAsync(Address entity)
    {
        await appDbContext.AddAsync(entity);
        await appDbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<Address> UpdateAsync(Address entity)
    {
        appDbContext.Update(entity);
        await appDbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<Address> DeleteAsync(Address entity)
    {
        appDbContext.Remove(entity);
        await appDbContext.SaveChangesAsync();
        return entity;
    }
}