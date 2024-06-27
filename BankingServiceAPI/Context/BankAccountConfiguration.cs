using BankingServiceAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingServiceAPI.Context;

public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.AccountNumber)
            .HasMaxLength(6)
            .IsRequired();
        
        builder.HasIndex(e => e.AccountNumber)
            .IsUnique();

        builder.Property(e => e.Agency)
            .HasMaxLength(4)
            .IsRequired();

        builder.Property(e => e.Balance)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.AccountType)
            .HasConversion(
                v => v.ToString(),
                v => (AccountType)Enum.Parse(typeof(AccountType), v))
            .IsRequired();
    }
}