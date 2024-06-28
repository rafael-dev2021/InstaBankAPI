using BankingServiceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingServiceAPI.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) 
{
    public DbSet<BankAccount> BankAccounts { get; init; }
    public DbSet<BankTransaction> BankTransactions { get; init; }
    public DbSet<TransactionLog> TransactionLogs { get; init; }
    public DbSet<User> Users { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}