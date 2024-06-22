using BankingServiceAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingServiceAPI.Context;

public class IndividualAccountConfiguration : IEntityTypeConfiguration<IndividualAccount>
{
    public void Configure(EntityTypeBuilder<IndividualAccount> builder)
    {
        builder.HasBaseType<BaseEntity>();
        builder.Property(e => e.Cpf)
            .IsRequired()
            .HasMaxLength(14);
                   
        builder.Property(e => e.DateOfBirth)
            .IsRequired();
    }
}