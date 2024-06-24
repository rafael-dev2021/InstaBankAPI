using BankingServiceAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingServiceAPI.Context;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(14)
            .IsRequired();

        builder.Property(x => x.Cpf)
            .HasMaxLength(14)
            .IsRequired();

        builder.HasIndex(u => u.Cpf)
            .IsUnique();

        builder.Property(x => x.Role)
            .HasMaxLength(15)
            .IsRequired();
    }
}