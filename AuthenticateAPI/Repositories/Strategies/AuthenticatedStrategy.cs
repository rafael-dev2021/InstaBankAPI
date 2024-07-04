using System.Text;
using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Exceptions;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;

namespace AuthenticateAPI.Repositories.Strategies;

public class AuthenticatedStrategy(SignInManager<User> signInManager, IDistributedCache cache) : IAuthenticatedStrategy
{
    private const string LoginAttemptsKeyPrefix = "login_attempts_";

    public async Task<AuthenticatedDtoResponse> AuthenticatedAsync(LoginDtoRequest request)
    {
        var loginAttemptsKey = $"{LoginAttemptsKeyPrefix}{request.Email}";
        var loginAttempts = await GetLoginAttemptsAsync(loginAttemptsKey);

        Log.Information("[AUTHENTICATION] Attempting to authenticate user [{Email}]", request.Email);

        if (loginAttempts >= 3)
        {
            Log.Warning("[AUTHENTICATION] User [{Email}] account is locked due to multiple failed attempts.",
                request.Email);
            return new AuthenticatedDtoResponse(false, "Your account is locked. Please contact support.");
        }

        var result = await signInManager.PasswordSignInAsync(request.Email!, request.Password!,
            isPersistent: request.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            await ResetLoginAttemptsAsync(loginAttemptsKey);
            Log.Information("[AUTHENTICATION] User [{Email}] successfully authenticated.", request.Email);
            return new AuthenticatedDtoResponse(true, "Login successful.");
        }

        await IncrementLoginAttemptsAsync(loginAttemptsKey);

        var errorMessage = result.IsLockedOut
            ? "Your account is locked. Please contact support."
            : "Invalid email or password. Please try again.";

        Log.Warning("[AUTHENTICATION] Failed authentication attempt for user [{Email}] with message: [{ErrorMessage}]", request.Email,
            errorMessage);
        return new AuthenticatedDtoResponse(false, errorMessage);
    }

    public async Task<int> GetLoginAttemptsAsync(string cacheKey)
    {
        try
        {
            var attempts = await cache.GetStringAsync(cacheKey);
            return attempts != null ? int.Parse(attempts) : 0;
        }
        catch (CacheException ex)
        {
            Log.Error(ex, "[CACHE] Error retrieving login attempts for cache key [{CacheKey}]", cacheKey);
            throw new CacheException($"[CACHE] Error retrieving login attempts for cache key [{cacheKey}]", ex);
        }
    }

    public async Task IncrementLoginAttemptsAsync(string cacheKey)
    {
        try
        {
            var attemptsBytes = await cache.GetAsync(cacheKey);
            var attempts = attemptsBytes != null ? int.Parse(Encoding.UTF8.GetString(attemptsBytes)) : 0;
            attempts++;
            await cache.SetStringAsync(cacheKey, attempts.ToString(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            Log.Information("[CACHE] Login attempts incremented for cache key [{CacheKey}]", cacheKey);
        }
        catch (CacheException ex)
        {
            Log.Error(ex, "[CACHE] Error incrementing login attempts for cache key [{CacheKey}]", cacheKey);
            throw new CacheException($"[CACHE] Error incrementing login attempts for cache key [{cacheKey}]", ex);
        }
    }

    public async Task ResetLoginAttemptsAsync(string cacheKey)
    {
        try
        {
            await cache.RemoveAsync(cacheKey);
            Log.Information("[CACHE] Login attempts reset for cache key [{CacheKey}]", cacheKey);
        }
        catch (CacheException ex)
        {
            Log.Error(ex, "[CACHE] Error resetting login attempts for cache key [{CacheKey}]", cacheKey);
            throw new CacheException($"[CACHE] Error resetting login attempts for cache key [{cacheKey}]", ex);
        }
    }
}