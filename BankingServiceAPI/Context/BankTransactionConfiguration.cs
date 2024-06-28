using BankingServiceAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingServiceAPI.Context;

public class BankTransactionConfiguration : IEntityTypeConfiguration<BankTransaction>
{
    public void Configure(EntityTypeBuilder<BankTransaction> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasDiscriminator<string>("TransactionType")
            .HasValue<Deposit>("Deposit")
            .HasValue<Transfer>("Transfer")
            .HasValue<Withdraw>("Withdraw");

        builder.Property(e => e.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(e => e.TransferDate)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.HasOne(e => e.AccountOrigin)
            .WithMany()
            .HasForeignKey(x => x.AccountOriginId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.AccountDestination)
            .WithMany()
            .HasForeignKey(x => x.AccountDestinationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("BankTransactions");
    }
}