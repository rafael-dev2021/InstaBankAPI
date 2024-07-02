using AuthenticateAPI.Context;
using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthenticateAPI.Repositories;

public class AuthenticatedRepository(
    SignInManager<User> signInManager,
    UserManager<User> userManager,
    IAuthenticatedStrategy authenticatedStrategy,
    IRegisterStrategy registerStrategy,
    IUpdateProfileStrategy updateProfileStrategy,
    AppDbContext appDbContext) :
    IAuthenticatedRepository
{
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var users = await appDbContext.Users.AsNoTracking().ToListAsync();

        var usersWithRoles = new List<User>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            var userWithRole = new User
            {
                Id = user.Id,
                Email = user.Email
            };
            userWithRole.SetName(user.Name);
            userWithRole.SetLastName(user.LastName);
            userWithRole.SetCpf(user.Cpf);
            userWithRole.SetPhoneNumber(user.PhoneNumber!);
            userWithRole.SetRole(roles.FirstOrDefault());

            usersWithRoles.Add(userWithRole);
        }

        return usersWithRoles;
    }

    public async Task<AuthenticatedDtoResponse> AuthenticateAsync(LoginDtoRequest loginDtoRequest)
    {
        return await authenticatedStrategy.AuthenticatedAsync(loginDtoRequest);
    }

    public async Task<RegisteredDtoResponse> RegisterAsync(RegisterDtoRequest request)
    {
        return await registerStrategy.CreateUserAsync(request);
    }

    public async Task<UpdatedDtoResponse> UpdateProfileAsync(UpdateUserDtoRequest updateUserDtoRequest, string userId)
    {
        return await updateProfileStrategy.UpdateProfileAsync(updateUserDtoRequest, userId);
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordDtoRequest changePasswordDtoRequest)
    {
        var user = await userManager.FindByEmailAsync(changePasswordDtoRequest.Email!);
        if (user == null) return false;
        var changePasswordResult =
            await userManager.ChangePasswordAsync(user, changePasswordDtoRequest.CurrentPassword!,
                changePasswordDtoRequest.NewPassword!);

        return changePasswordResult.Succeeded;
    }

    public async Task<User?> GetUserProfileAsync(string userEmail)
    {
        var user = await userManager.FindByEmailAsync(userEmail);
        return user ?? null;
    }
    
    public async Task<User?> GetUserIdProfileAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user ?? null;
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
    
    public async Task SaveAsync(User user)
    {
        appDbContext.Update(user);
        await appDbContext.SaveChangesAsync();
    }
}