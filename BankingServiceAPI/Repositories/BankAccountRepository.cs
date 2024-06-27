using BankingServiceAPI.Algorithms.Interfaces;
using BankingServiceAPI.Context;
using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingServiceAPI.Repositories;

public class BankAccountRepository(
    AppDbContext context,
    IAccountNumberGenerator accountNumberGenerator) : IBankAccountRepository
{
    public async Task<IEnumerable<BankAccount>> GetEntitiesAsync() =>
        await context.BankAccounts
            .Include(x => x.User)
            .AsNoTracking()
            .ToListAsync();

    public async Task<BankAccount?> GetEntityByIdAsync(int? id) =>
        await context.BankAccounts
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<BankAccount> CreateEntityAsync(BankAccount entity)
    {
        entity.SetAccountNumber(await accountNumberGenerator.GenerateAccountNumberAsync());
        entity.SetAgency(await accountNumberGenerator.GenerateAgencyNumberAsync());

        await context.BankAccounts.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteEntityAsync(int? id)
    {
        var account = await GetEntityByIdAsync(id);
        if (account == null)
            throw new GetIdNotFoundException($"Id: {id} not found.");

        context.BankAccounts.Remove(account);
        await context.SaveChangesAsync();
        return true;
    }
}