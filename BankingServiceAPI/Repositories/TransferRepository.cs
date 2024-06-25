using BankingServiceAPI.Context;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingServiceAPI.Repositories;

public class TransferRepository(AppDbContext context) : ITransferRepository
{
    public async Task<BankAccount?> GetByAccountNumberAsync(int accountNumber)
    {
        return await context.BankAccounts
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
    }

    public async Task<BankAccount?> GetByCpfAsync(string cpf)
    {
        return await context.BankAccounts
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.User!.Cpf == cpf);
    }
}