﻿using AuthenticateAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthenticateAPI.Context;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Role).HasMaxLength(10).IsRequired();

        builder.Property(x => x.Cpf)
            .HasMaxLength(14)
            .IsRequired();

        builder.HasIndex(e => e.Cpf)
            .IsUnique();

        builder.Property(x => x.Email)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(e => e.Email)
            .IsUnique();

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(14)
            .IsRequired();

        builder.HasIndex(e => e.PhoneNumber)
            .IsUnique();

        builder.HasMany(u => u.Tokens)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId);
    }
}