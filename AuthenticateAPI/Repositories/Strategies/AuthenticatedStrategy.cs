using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;

namespace AuthenticateAPI.Repositories.Strategies;

public class AuthenticatedStrategy(SignInManager<User> signInManager, IDistributedCache cache) : IAuthenticatedStrategy
{
    private const string LoginAttemptsKeyPrefix = "login_attempts_";

    public async Task<AuthenticatedDtoResponse> AuthenticatedAsync(LoginDtoRequest request)
    {
        var loginAttemptsKey = $"{LoginAttemptsKeyPrefix}{request.Email}";
        var loginAttempts = await GetLoginAttemptsAsync(loginAttemptsKey);

        if (loginAttempts >= 5)
        {
            return new AuthenticatedDtoResponse(false, "Your account is locked. Please contact support.");
        }

        var result = await signInManager.PasswordSignInAsync(request.Email!, request.Password!,
            isPersistent: request.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            await ResetLoginAttemptsAsync(loginAttemptsKey);
            return new AuthenticatedDtoResponse(true, "Login successful.");
        }

        await IncrementLoginAttemptsAsync(loginAttemptsKey);
        
        var errorMessage = result.IsLockedOut ? "Your account is locked. Please contact support." :
            "Invalid email or password. Please try again.";
        return new AuthenticatedDtoResponse(false, errorMessage);
    }

    private async Task<int> GetLoginAttemptsAsync(string cacheKey)
    {
        var attempts = await cache.GetStringAsync(cacheKey);
        return attempts != null ? int.Parse(attempts) : 0;
    }

    private async Task IncrementLoginAttemptsAsync(string cacheKey)
    {
        var attempts = await GetLoginAttemptsAsync(cacheKey);
        attempts++;
        await cache.SetStringAsync(cacheKey, attempts.ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });
    }

    private async Task ResetLoginAttemptsAsync(string cacheKey)
    {
        await cache.RemoveAsync(cacheKey);
    }
}
