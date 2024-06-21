using AuthenticateAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthenticateAPI.Context;

public class TokenConfiguration : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.TokenValue);
        builder.Property(t => t.TokenRevoked);
        builder.Property(t => t.TokenExpired);
        
        builder.HasOne(t => t.User)
            .WithMany(u => u.Tokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(t => t.TokenType)
            .HasConversion(
                v => v.ToString(),
                v => (TokenType)Enum.Parse(typeof(TokenType), v))
            .IsRequired();
    }
}