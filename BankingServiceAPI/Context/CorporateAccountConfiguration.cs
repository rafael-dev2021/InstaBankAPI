using BankingServiceAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingServiceAPI.Context;

public class CorporateAccountConfiguration : IEntityTypeConfiguration<CorporateAccount>
{
    public void Configure(EntityTypeBuilder<CorporateAccount> builder)
    {
        builder.HasBaseType<BaseEntity>();
            
        builder.Property(e => e.Cnpj)
            .IsRequired()
            .HasMaxLength(18);

        builder.Property(e => e.BusinessName)
            .IsRequired()
            .HasMaxLength(100);
    }
}