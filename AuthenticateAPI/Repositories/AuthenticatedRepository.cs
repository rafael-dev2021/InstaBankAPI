using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Strategies;
using Microsoft.AspNetCore.Identity;

namespace AuthenticateAPI.Repositories;

public class AuthenticatedRepository(
    SignInManager<User> signInManager,
    UserManager<User> userManager,
    IAuthenticatedStrategy authenticatedStrategy,
    IRegisterStrategy registerStrategy,
    IUpdateProfileStrategy updateProfileStrategy) :
    IAuthenticatedRepository
{
    public async Task<AuthenticatedDtoResponse> AuthenticateAsync(LoginDtoRequest loginDtoRequest)
    {
        return await authenticatedStrategy.AuthenticatedAsync(loginDtoRequest);
    }

    public async Task<RegisteredDtoResponse> RegisterAsync(RegisterDtoRequest request)
    {
        var validationErrors = await registerStrategy.ValidateAsync(request.Cpf, request.Email, request.PhoneNumber);

        if (validationErrors.Count == 0)
            return await registerStrategy.CreateUserAsync(request);

        var errorMessage = string.Join(Environment.NewLine, validationErrors);
        return new RegisteredDtoResponse(false, errorMessage);
    }

    public async Task<UpdatedDtoResponse> UpdateProfileAsync(UpdateUserDtoRequest updateUserDtoRequest, string userId)
    {
        return await updateProfileStrategy.UpdateProfileAsync(updateUserDtoRequest, userId);
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordDtoRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email!);
        if (user == null) return false;
        var changePasswordResult =
            await userManager.ChangePasswordAsync(user, request.CurrentPassword!, request.NewPassword!);

        return changePasswordResult.Succeeded;
    }

    public async Task<User?> GetUserProfileAsync(string userEmail)
    {
        var user = await userManager.FindByEmailAsync(userEmail);
        if (user == null) return null;

        var userProfile = new User();
        userProfile.SetName(user.Name);
        userProfile.SetLastName(user.LastName);
        userProfile.SetEmail(user.Email);
        userProfile.SetPhoneNumber(user.PhoneNumber!);

        return userProfile;
    }

    public async Task<bool> ForgotPasswordAsync(string email, string newPassword)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null) return false;

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetPasswordResult = await userManager.ResetPasswordAsync(user, token, newPassword);

        return resetPasswordResult.Succeeded;
    }

    public async Task LogoutAsync()
    {
        await signInManager.SignOutAsync();
    }
}