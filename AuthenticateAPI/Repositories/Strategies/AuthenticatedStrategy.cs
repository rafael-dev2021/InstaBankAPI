﻿using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AuthenticateAPI.Repositories.Strategies;

public class AuthenticatedStrategy(SignInManager<User> signInManager) : IAuthenticatedStrategy
{
    public async Task<AuthenticatedDtoResponse> AuthenticatedAsync(LoginDtoRequest request)
    {
        var result = await signInManager.PasswordSignInAsync(request.Email!, request.Password!,
            isPersistent: request.RememberMe, lockoutOnFailure: true);

        var errorMessages = new Dictionary<Func<SignInResult, bool>, string>
        {
            { r => r.Succeeded, "Login successful." },
            { r => r.IsLockedOut, "Your account is locked. Please contact support." },
            { _ => true, "Invalid email or password. Please try again." }
        };

        var message = errorMessages.First(kv => kv.Key(result)).Value;

        return new AuthenticatedDtoResponse(result.Succeeded, message);
    }
}