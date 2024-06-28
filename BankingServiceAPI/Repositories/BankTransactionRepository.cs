using BankingServiceAPI.Context;
using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingServiceAPI.Repositories;

public class BankTransactionRepository(AppDbContext context) : IBankTransactionRepository
{
    public async Task<IEnumerable<BankTransaction>> GetEntitiesAsync()
    {
        return await context.BankTransactions
            .AsNoTracking()
            .Include(t => t.AccountOrigin)
            .Include(t => t.AccountDestination)
            .ToListAsync();
    }

    public async Task<BankTransaction?> GetEntityByIdAsync(int? id)
    {
        return await context.BankTransactions
            .Include(t => t.AccountOrigin)
            .Include(t => t.AccountDestination)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<BankTransaction> CreateEntityAsync(BankTransaction entity)
    {
        await context.BankTransactions.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteEntityAsync(int? id)
    {
        var transaction = await GetEntityByIdAsync(id);
        if (transaction == null)
            throw new GetIdNotFoundException(
                $"Id: {id} not found.");

        context.BankTransactions.Remove(transaction);
        await context.SaveChangesAsync();
        return true;
    }
}