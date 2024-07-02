using AuthenticateAPI.Context;
using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AuthenticateAPI.Repositories.Strategies;

public class UpdateProfileStrategy(UserManager<User> userManager, AppDbContext appDbContext) : IUpdateProfileStrategy
{
    public async Task<UpdatedDtoResponse> UpdateProfileAsync(UpdateUserDtoRequest updateUserDtoRequest, string userId)
    {
        Log.Information("[PROFILE UPDATE] Attempting to update profile for user [{UserId}]", userId);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            Log.Warning("[PROFILE UPDATE] User [{UserId}] not found.", userId);
            return new UpdatedDtoResponse(false, "User not found");
        }

        var validationErrors = await ValidateUserDetailsAsync(updateUserDtoRequest, user);
        if (validationErrors.Count != 0)
        {
            LogValidationErrors(userId, validationErrors);
            return new UpdatedDtoResponse(false, string.Join(Environment.NewLine, validationErrors));
        }

        UpdateUserFields(user, updateUserDtoRequest);
        var result = await userManager.UpdateAsync(user);

        return result.Succeeded ? LogAndReturnSuccess(userId) : LogAndReturnFailure(userId);
    }

    private async Task<List<string>> ValidateUserDetailsAsync(UpdateUserDtoRequest updateUserDtoRequest, User user)
    {
        var validationErrors = new List<string>();

        if (updateUserDtoRequest.Email != user.Email &&
            await IsEmailAlreadyUsedByAnotherUserAsync(updateUserDtoRequest.Email!, user.Id))
            validationErrors.Add("Email already used by another user.");

        if (updateUserDtoRequest.PhoneNumber != user.PhoneNumber &&
            await IsPhoneNumberAlreadyUsedByAnotherUserAsync(updateUserDtoRequest.PhoneNumber!, user.Id))
            validationErrors.Add("Phone number already used by another user.");

        return validationErrors;
    }

    private async Task<bool> IsEmailAlreadyUsedByAnotherUserAsync(string email, string userId) =>
        await appDbContext.Users.AnyAsync(x => x.Email == email && x.Id != userId);

    private async Task<bool> IsPhoneNumberAlreadyUsedByAnotherUserAsync(string phone, string userId) =>
        await appDbContext.Users.AnyAsync(x => x.PhoneNumber == phone && x.Id != userId);

    private static void UpdateUserFields(User user, UpdateUserDtoRequest updateUserDtoRequest)
    {
        UpdateField(user, (u, v) => u.Email = v, user.Email, updateUserDtoRequest.Email);
        UpdateField(user, (u, v) => u.PhoneNumber = v, user.PhoneNumber, updateUserDtoRequest.PhoneNumber);
        UpdateField(user, (u, v) => u.SetName(v), user.Name, updateUserDtoRequest.Name);
        UpdateField(user, (u, v) => u.SetLastName(v), user.LastName, updateUserDtoRequest.LastName);
    }

    private static void UpdateField<T>(User user, Action<User, T> setter, T currentValue, T newValue)
    {
        if (!EqualityComparer<T>.Default.Equals(newValue, default) &&
            !EqualityComparer<T>.Default.Equals(newValue, currentValue))
        {
            setter(user, newValue);
        }
    }

    private static void LogValidationErrors(string userId, List<string> validationErrors)
    {
        Log.Warning("[PROFILE UPDATE] Validation errors for user [{UserId}] with message: [{Errors}]", userId,
            string.Join(", ", validationErrors));
    }

    private static UpdatedDtoResponse LogAndReturnSuccess(string userId)
    {
        Log.Information("[PROFILE UPDATE] Profile successfully updated for user [{UserId}]", userId);
        return new UpdatedDtoResponse(true, "Profile updated successfully.");
    }

    private static UpdatedDtoResponse LogAndReturnFailure(string userId)
    {
        Log.Warning("[PROFILE UPDATE] Failed to update profile for user [{UserId}]", userId);
        return new UpdatedDtoResponse(false, "Failed to update profile.");
    }
}