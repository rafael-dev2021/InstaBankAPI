using BankingServiceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingServiceAPI.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<IndividualAccount> IndividualAccounts { get; init; }
    public DbSet<CorporateAccount> CorporateAccounts { get; init; }
    public DbSet<Address> Addresses { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}