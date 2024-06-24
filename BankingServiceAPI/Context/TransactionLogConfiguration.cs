using BankingServiceAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingServiceAPI.Context;

public class TransactionLogConfiguration : IEntityTypeConfiguration<TransactionLog>
{
    public void Configure(EntityTypeBuilder<TransactionLog> builder)
    {
        builder.HasKey(e => e.TransactionId);

        builder.Property(e => e.TimeDate)
            .IsRequired();

        builder.HasOne(e => e.BankTransaction)
            .WithMany()
            .HasForeignKey(x=>x.BankTransactionId);

        builder.ToTable("TransactionLogs");
    }
}