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

    public async Task<RegisteredDtoResponse> RegisterAsync(RegisterDtoRequest request)
    {
        var validationErrors = await _registerStrategy.ValidateAsync(request.Cpf, request.Email, request.PhoneNumber);

        if (validationErrors.Count == 0)
            return await _registerStrategy.CreateUserAsync(request);

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

    public async Task<User> GetUserProfileAsync(string userEmail)
    {
        var user = await userManager.FindByEmailAsync(userEmail);

        var userProfile = new User();
        userProfile.SetName(user?.Name);
        userProfile.SetLastName(user?.LastName);
        userProfile.SetEmail(user?.Email);
        userProfile.SetPhoneNumber(user?.PhoneNumber!);

        return userProfile;
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