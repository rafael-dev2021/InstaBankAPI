using BankingServiceAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingServiceAPI.Context;

public class BaseEntityConfiguration : IEntityTypeConfiguration<BaseEntity>
{
    public void Configure(EntityTypeBuilder<BaseEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.AccountNumber)
            .IsRequired()
            .HasMaxLength(9);

        builder.Property(e => e.Agency)
            .IsRequired()
            .HasMaxLength(4);

        builder.Property(e => e.Balance)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.HasOne(e => e.Address)
            .WithOne()
            .HasForeignKey<BaseEntity>(e => e.AddressId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}