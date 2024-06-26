using BankingServiceAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingServiceAPI.Context;

public class TransactionLogConfiguration : IEntityTypeConfiguration<TransactionLog>
{
    public void Configure(EntityTypeBuilder<TransactionLog> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.TransactionDate)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(e => e.TransactionType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Description)
            .HasMaxLength(250);

        builder.Property(e => e.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne(e => e.BankTransaction)
            .WithMany()
            .HasForeignKey(e => e.BankTransactionId);

        builder.ToTable("TransactionLogs");
    }
}