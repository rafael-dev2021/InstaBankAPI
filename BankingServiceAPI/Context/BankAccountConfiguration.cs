using BankingServiceAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingServiceAPI.Context;

public class AccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.HasKey(e => e.AccountNumber);
        
        builder.Property(e => e.AccountNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Agency)
            .IsRequired()
            .HasMaxLength(4);

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