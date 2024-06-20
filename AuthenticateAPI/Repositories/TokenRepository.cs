using AuthenticateAPI.Context;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthenticateAPI.Repositories;

public class TokenRepository(AppDbContext context) : ITokenRepository
{
    public async Task<List<Token>> FindAllValidTokenByUser(string userId)
    {
        return await context.Tokens
            .Where(t => t.UserId == userId && (!t.TokenExpired || !t.TokenRevoked))
            .ToListAsync();
    }

    public async Task<List<Token>> FindAllTokensByUserId(string userId)
    {
        return await context.Tokens
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public async Task<Token?> FindByTokenValue(string token)
    {
        return await context.Tokens
            .FirstOrDefaultAsync(t => t.TokenValue == token);
    }

    public async Task DeleteByUser(User user)
    {
        var tokens = await context.Tokens
            .Where(t => t.UserId == user.Id)
            .ToListAsync();

        context.Tokens.RemoveRange(tokens);
        await context.SaveChangesAsync();
    }

    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task SaveAllTokensAsync(IEnumerable<Token> tokens)
    {
        context.Tokens.UpdateRange(tokens);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAllTokensAsync(IEnumerable<Token> tokens)
    {
        context.Tokens.RemoveRange(tokens);
        await context.SaveChangesAsync();
    }

    public async Task<Token> SaveTokenAsync(Token token)
    {
        var existingToken = await context.Tokens.FindAsync(token.Id);
        if (existingToken == null)
        {
            context.Tokens.Add(token); 
        }
        else
        {
            context.Entry(existingToken).CurrentValues.SetValues(token); 
        }
        await context.SaveChangesAsync();
        return token;
    }

}