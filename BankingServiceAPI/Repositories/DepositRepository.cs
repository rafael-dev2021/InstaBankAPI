using BankingServiceAPI.Context;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingServiceAPI.Repositories;

public class DepositRepository(AppDbContext context) : IDepositRepository
{
    public async Task<BankAccount?> GetByAccountNumberAsync(int accountNumber)
    {
        return await context.BankAccounts
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
    }
}