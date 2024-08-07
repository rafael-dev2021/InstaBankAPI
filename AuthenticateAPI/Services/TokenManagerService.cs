﻿using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using AuthenticateAPI.Security;
using Serilog;

namespace AuthenticateAPI.Services;

public class TokenManagerService(
    ITokenService tokenService,
    ITokenRepository tokenRepository,
    IAuthenticatedRepository authenticatedRepository) : ITokenManagerService
{
    public async Task<TokenDtoResponse> GenerateTokenResponseAsync(User user)
    {
        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user);

        await ClearTokensAsync(user.Id);
        var accessTokenToken = await SaveUserTokenAsync(user, accessToken);
        var refreshTokenToken = await SaveUserTokenAsync(user, refreshToken);

        user.Tokens.Clear();
        user.Tokens.Add(accessTokenToken);
        user.Tokens.Add(refreshTokenToken);
        await authenticatedRepository.SaveAsync(user);

        Log.Information("[TOKENS] Tokens generated successfully for user [{UserId}]", user.Id);

        return new TokenDtoResponse(accessTokenToken.TokenValue!, refreshTokenToken.TokenValue!);
    }

    public void RevokeAllUserTokens(User user)
    {
        var validUserTokens = tokenRepository.FindAllValidTokenByUser(user.Id).Result;
        if (validUserTokens.Count == 0) return;
        foreach (var token in validUserTokens)
        {
            token.SetTokenExpired(true);
            token.SetTokenRevoked(true);
        }

        tokenRepository.SaveAllTokensAsync(validUserTokens).Wait();

        Log.Information("[TOKENS] All tokens revoked for user [{UserId}]", user.Id);
    }

    public async Task<bool> RevokedTokenAsync(string token)
    {
        var dbToken = await tokenRepository.FindByTokenValue(token);
        return dbToken is { TokenRevoked: true };
    }

    public async Task<bool> ExpiredTokenAsync(string token)
    {
        var dbToken = await tokenRepository.FindByTokenValue(token);
        return dbToken is { TokenExpired: true };
    }

    public async Task ClearTokensAsync(string userId)
    {
        var tokens = await tokenRepository.FindAllTokensByUserId(userId);
        if (tokens.Count == 0) return;
        await tokenRepository.DeleteAllTokensAsync(tokens);
        Log.Information("[CLEARED TOKENS] Cleared tokens for user [{UserId}]", userId);
    }

    public async Task<Token> SaveUserTokenAsync(User user, string jwtToken)
    {
        var token = new Token();
        token.SetTokenValue(jwtToken);
        token.SetTokenType(TokenType.Bearer);
        token.SetTokenExpired(false);
        token.SetTokenRevoked(false);
        token.SetUserId(user.Id);
        token.SetUser(user);

        var savedToken = await tokenRepository.SaveTokenAsync(token);
        Log.Information("[TOKENS] Token saved successfully for user [{UserId}]", user.Id);

        return savedToken;
    }
}