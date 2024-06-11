using AuthenticateAPI.Context;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthenticateAPI.Repositories.Strategies;

public class RegisterStrategy(
    SignInManager<User> signInManager,
    UserManager<User> userManager,
    AppDbContext appDbContext)
{
    private async Task<bool> IsCpfAlreadyUsed(string cpf) =>
        await appDbContext.Users.FirstOrDefaultAsync(x => x.Cpf == cpf) != null;

    private async Task<bool> IsEmailAlreadyUsed(string email) =>
        await appDbContext.Users.FirstOrDefaultAsync(x => x.Email == email) != null;

    private async Task<bool> IsPhoneNumberAlreadyUsed(string phone) =>
        await appDbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phone) != null;

    public async Task<List<string>> ValidateAsync(string? cpf, string? email, string? phoneNumber)
    {
        var validationErrors = new List<string>();

        var errors = new Dictionary<Func<Task<bool>>, string>
        {
            { () => IsCpfAlreadyUsed(cpf!), "CPF already used." },
            { () => IsEmailAlreadyUsed(email!), "Email already used." },
            { () => IsPhoneNumberAlreadyUsed(phoneNumber!), "Phone number already used." }
        };

        foreach (var error in errors)
        {
            if (await error.Key())
            {
                validationErrors.Add(error.Value);
            }
        }

        return validationErrors;
    }

    public async Task<RegisteredDtoResponse> CreateUserAsync(User user, string password)
    {
        var validationErrors = await ValidateAsync(user.Cpf, user.Email, user.PhoneNumber);

        if (validationErrors.Count > 0)
        {
            return new RegisteredDtoResponse(false, string.Join(", ", validationErrors));
        }

        var appUser = new User
        {
            Email = user.Email,
            UserName = user.Email,
            PhoneNumber = user.PhoneNumber
        };
        appUser.SetName(user.Name);
        appUser.SetLastName(user.LastName);
        appUser.SetCpf(user.Cpf);

        var result = await userManager.CreateAsync(appUser, password);

        if (!result.Succeeded) return new RegisteredDtoResponse(false, "Registration failed.");

        await userManager.AddToRoleAsync(appUser, "User");
        await signInManager.SignInAsync(appUser, isPersistent: false);
        return new RegisteredDtoResponse(true, "Registration successful.");
    }
}