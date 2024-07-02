using AuthenticateAPI.Context;
using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AuthenticateAPI.Repositories.Strategies;

public class RegisterStrategy(
    SignInManager<User> signInManager,
    UserManager<User> userManager,
    AppDbContext appDbContext) : IRegisterStrategy
{
    public async Task<RegisteredDtoResponse> CreateUserAsync(RegisterDtoRequest request)
    {
        Log.Information("[REGISTRATION] Attempting to register user with Email= [{Email}]", request.Email);

        if (!IsPasswordConfirmed(request))
        {
            Log.Warning("[REGISTRATION] Password and confirm password do not match for Email= [{Email}]",
                request.Email);
            return new RegisteredDtoResponse(false, "Password and confirm password do not match.");
        }

        var validationErrors = await ValidateUserDetailsAsync(request.Cpf, request.Email, request.PhoneNumber);

        if (validationErrors.Count != 0)
        {
            return new RegisteredDtoResponse(false, string.Join(", ", validationErrors));
        }

        var appUser = CreateUser(request);
        var result = await userManager.CreateAsync(appUser, request.Password);

        if (!result.Succeeded)
        {
            Log.Warning("[REGISTRATION] User creation failed for Email= [{Email}]", request.Email);
            return new RegisteredDtoResponse(false, "Registration failed.");
        }

        await AssignUserRoleAndSignInAsync(appUser);
        Log.Information("[REGISTRATION] User registered and signed in successfully with Email= [{Email}]",
            request.Email);
        return new RegisteredDtoResponse(true, "Registration successful.");
    }

    private static bool IsPasswordConfirmed(RegisterDtoRequest request)
    {
        return request.Password == request.ConfirmPassword;
    }

    private async Task<List<string>> ValidateUserDetailsAsync(string? cpf, string? email, string? phoneNumber)
    {
        Log.Information(
            "[REGISTRATION] Validating user details: CPF= [{Cpf}], Email= [{Email}], PhoneNumber= [{PhoneNumber}]",
            cpf, email, phoneNumber);

        var validationErrors = new List<string>();

        if (await IsCpfAlreadyUsedAsync(cpf!)) validationErrors.Add("[CPF already used]");
        if (await IsEmailAlreadyUsedAsync(email!)) validationErrors.Add("[Email already used]");
        if (await IsPhoneNumberAlreadyUsedAsync(phoneNumber!)) validationErrors.Add("[Phone number already used]");

        if (validationErrors.Count != 0)
        {
            Log.Warning("[REGISTRATION] Validation errors: {Errors}.", string.Join(", ", validationErrors));
        }
        else
        {
            Log.Information("[REGISTRATION] Validation successful.");
        }

        return validationErrors;
    }

    private static User CreateUser(RegisterDtoRequest request)
    {
        var appUser = new User
        {
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber
        };
        appUser.SetName(request.Name);
        appUser.SetLastName(request.LastName);
        appUser.SetCpf(request.Cpf);
        appUser.SetRole("User");

        return appUser;
    }

    private async Task AssignUserRoleAndSignInAsync(User appUser)
    {
        await userManager.AddToRoleAsync(appUser, "User");
        await signInManager.SignInAsync(appUser, isPersistent: false);
    }

    private async Task<bool> IsCpfAlreadyUsedAsync(string cpf) =>
        await appDbContext.Users.FirstOrDefaultAsync(x => x.Cpf == cpf) != null;

    private async Task<bool> IsEmailAlreadyUsedAsync(string email) =>
        await appDbContext.Users.FirstOrDefaultAsync(x => x.Email == email) != null;

    private async Task<bool> IsPhoneNumberAlreadyUsedAsync(string phone) =>
        await appDbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phone) != null;
}