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

        builder.Property(e => e.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne(e => e.BankTransaction)
            .WithMany()
            .HasForeignKey(e => e.BankTransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(e => e.TransactionDetails, details =>
        {
            details.Property(d => d.TransactionStatus)
                .HasMaxLength(50)
                .IsRequired();

            details.Property(d => d.Description)
                .HasMaxLength(250);

            details.Property(d => d.Remarks)
                .HasMaxLength(500);

            details.Property(d => d.TransactionReference)
                .HasMaxLength(100);

            details.Property(d => d.Channel)
                .HasMaxLength(50);

            details.Property(d => d.ErrorDetails)
                .HasMaxLength(500);
        });

        builder.OwnsOne(e => e.TransactionAudit, audit =>
        {
            audit.Property(a => a.InitiatedBy)
                .HasMaxLength(50);

            audit.Property(a => a.ApprovedBy)
                .HasMaxLength(50)
                .IsRequired();

            audit.Property(a => a.IpAddress)
                .HasMaxLength(45);

            audit.Property(a => a.DeviceId)
                .HasMaxLength(100);

            audit.Property(a => a.Location)
                .HasMaxLength(250);

            audit.Property(a => a.Timestamp)
                .IsRequired()
                .HasColumnType("datetime2");
        });

        builder.ToTable("TransactionLogs");
    }
}