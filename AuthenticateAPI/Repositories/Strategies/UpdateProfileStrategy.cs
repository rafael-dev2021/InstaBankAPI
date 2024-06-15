using AuthenticateAPI.Context;
using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthenticateAPI.Repositories.Strategies;

public class UpdateProfileStrategy(UserManager<User> userManager, AppDbContext appDbContext) : IUpdateProfileStrategy
{
    private async Task<bool> IsEmailAlreadyUsedByAnotherUser(string email, string userId) =>
        await appDbContext.Users.AnyAsync(x => x.Email == email && x.Id != userId);

    private async Task<bool> IsPhoneNumberAlreadyUsedByAnotherUser(string phone, string userId) =>
        await appDbContext.Users.AnyAsync(x => x.PhoneNumber == phone && x.Id != userId);

    public async Task<List<string>> ValidateAsync(string? email, string? phoneNumber, string userId)
    {
        var validationErrors = new List<string>();

        var errors = new Dictionary<Func<Task<bool>>, string>
        {
            { () => IsEmailAlreadyUsedByAnotherUser(email!, userId), "Email already used by another user." },
            { () => IsPhoneNumberAlreadyUsedByAnotherUser(phoneNumber!, userId), "Phone number already used by another user." }
        };

        foreach (var error in errors)
        {
            if (!string.IsNullOrEmpty(error.Value) && await error.Key())
            {
                validationErrors.Add(error.Value);
            }
        }

        return validationErrors;
    }

    public async Task<UpdatedDtoResponse> UpdateProfileAsync(UpdateUserDtoRequest updateUserDtoRequest, string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        var validationErrors = await ValidateAsync(
            updateUserDtoRequest.Email != user!.Email ? updateUserDtoRequest.Email : null,
            updateUserDtoRequest.PhoneNumber != user.PhoneNumber ? updateUserDtoRequest.PhoneNumber : null,
            userId
        );
        
        if (validationErrors.Count > 0)
        {
            return new UpdatedDtoResponse(false, string.Join(Environment.NewLine, validationErrors));
        }

        UpdateField(user, (u, v) => u.Email = v, user.Email, updateUserDtoRequest.Email);
        UpdateField(user, (u, v) => u.PhoneNumber = v, user.PhoneNumber, updateUserDtoRequest.PhoneNumber);
        UpdateField(user, (u, v) => u.SetName(v), user.Name, updateUserDtoRequest.Name);
        UpdateField(user, (u, v) => u.SetLastName(v), user.LastName, updateUserDtoRequest.LastName);


        var result = await userManager.UpdateAsync(user);

        return result.Succeeded
            ? new UpdatedDtoResponse(true, "Profile updated successfully.")
            : new UpdatedDtoResponse(false, "Failed to update profile.");
    }
    
    private static void UpdateField<T>(User user, Action<User, T> setter, T currentValue, T newValue)
    {
        if (!EqualityComparer<T>.Default.Equals(newValue, default) && !EqualityComparer<T>.Default.Equals(newValue, currentValue))
        {
            setter(user, newValue);
        }
    }
}