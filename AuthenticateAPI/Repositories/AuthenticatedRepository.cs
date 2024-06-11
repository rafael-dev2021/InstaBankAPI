using AuthenticateAPI.Context;
using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Strategies;
using Microsoft.AspNetCore.Identity;

namespace AuthenticateAPI.Repositories;

public class AuthenticatedRepository(
    SignInManager<User> signInManager,
    UserManager<User> userManager,
    AppDbContext appDbContext) :
    IAuthenticatedRepository
{
    private readonly AuthenticatedStrategy _authenticationStrategy = new(signInManager);

    private readonly RegisterStrategy _registerStrategy = new(
        signInManager,
        userManager,
        appDbContext
    );

    public async Task<AuthenticatedDtoResponse> AuthenticateAsync(LoginDtoRequest loginDtoRequest)
    {
        return await _authenticationStrategy.AuthenticatedAsync(loginDtoRequest);
    }

    public async Task<RegisteredDtoResponse> RegisterAsync(User user, string password)
    {
        var validationErrors = await _registerStrategy.ValidateAsync(user.Cpf, user.Email, user.PhoneNumber);

        if (validationErrors.Count == 0)
            return await _registerStrategy.CreateUserAsync(user, password);

        var errorMessage = string.Join(Environment.NewLine, validationErrors);
        return new RegisteredDtoResponse(false, errorMessage);
    }


    public Task<(bool success, string errorMessage)> UpdateProfileAsync(UpdateUserDtoRequest updateUserDtoRequest)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ChangePasswordAsync(ChangePasswordDtoRequest changePasswordDtoRequest)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetUserProfileAsync(string userEmail)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ForgotPasswordAsync(string email, string newPassWord)
    {
        throw new NotImplementedException();
    }

    public async Task LogoutAsync()
    {
        await signInManager.SignOutAsync();
    }
}