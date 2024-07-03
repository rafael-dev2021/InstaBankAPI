using System.Security.Claims;
using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Exceptions;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using AuthenticateAPI.Security;
using AutoMapper;
using Serilog;

namespace AuthenticateAPI.Services;

public class AuthenticateService(
    IAuthenticatedRepository repository,
    ITokenService tokenService,
    IMapper mapper,
    ITokenManagerService tokenManagerService) : IAuthenticateService
{
    private const string Succeeded = "succeeded";
    private const string Failed = "failed";

    public async Task<IEnumerable<UserDtoResponse>> GetAllUsersServiceAsync()
    {
        Log.Information("[USERS] Fetching all users DTOs.");
        var usersDto = await repository.GetAllUsersAsync();

        var enumerable = usersDto as User[] ?? usersDto.ToArray();
        if (enumerable.Length == 0)
        {
            Log.Information("[USERS] No users found.");
            return [];
        }

        var mappedUsers = mapper.Map<IEnumerable<UserDtoResponse>>(usersDto);
        Log.Information("[USERS] Successfully fetched and mapped [{UserCount}] users.", enumerable.Length);
        return mappedUsers;
    }

    public async Task<TokenDtoResponse> LoginServiceAsync(LoginDtoRequest request)
    {
        Log.Information("[LOGIN] Attempting to authenticate user with email: [{Email}]", request.Email);

        var authResponse = await repository.AuthenticateAsync(request);
        if (!authResponse.Success)
        {
            Log.Warning("[LOGIN] Authentication failed for email: [{Email}] with message: [{Message}]", request.Email,
                authResponse.Message);
            throw new UnauthorizedAccessException(authResponse.Message);
        }

        var user = await repository.GetUserProfileAsync(request.Email!);
        tokenManagerService.RevokeAllUserTokens(user!);

        var tokenResponse = await tokenManagerService.GenerateTokenResponseAsync(user!);
        Log.Information("[LOGIN] Login successful for email: [{Email}]", request.Email);
        return tokenResponse;
    }

    public async Task<TokenDtoResponse> RegisterServiceAsync(RegisterDtoRequest request)
    {
        Log.Information("[REGISTER] Attempting to register user with email: [{Email}]", request.Email);

        var registerResponse = await repository.RegisterAsync(request);
        if (!registerResponse.Success)
        {
            Log.Warning("[REGISTER] Registration failed for email: [{Email}] with message: [{Message}]", request.Email,
                registerResponse.Message);
            throw new UnauthorizedAccessException(registerResponse.Message);
        }

        var user = await repository.GetUserProfileAsync(request.Email!);
        var tokenResponse = await tokenManagerService.GenerateTokenResponseAsync(user!);
        Log.Information("[REGISTER] Registration successful for email: [{Email}]", request.Email);
        return tokenResponse;
    }

    public async Task<TokenDtoResponse> UpdateUserServiceAsync(UpdateUserDtoRequest request, string userId)
    {
        Log.Information("[UPDATE_USER] Attempting to update profile for userId: [{UserId}]", userId);

        var updateResponse = await repository.UpdateProfileAsync(request, userId);
        if (!updateResponse.Success)
        {
            Log.Warning("[UPDATE_USER] Profile update failed for userId: [{UserId}] with message: [{Message}]", userId,
                updateResponse.Message);
            throw new UnauthorizedAccessException(updateResponse.Message);
        }

        var user = await repository.GetUserProfileAsync(request.Email!);
        tokenManagerService.RevokeAllUserTokens(user!);

        var tokenResponse = await tokenManagerService.GenerateTokenResponseAsync(user!);
        Log.Information("[UPDATE_USER] Profile update successful for userId: [{UserId}]", userId);
        return tokenResponse;
    }

    public async Task<bool> ChangePasswordServiceAsync(ChangePasswordDtoRequest request, string userId)
    {
        Log.Information("[CHANGE_PASSWORD] Attempting to change password for userId: [{UserId}]", userId);

        var user = await repository.GetUserIdProfileAsync(userId);
        if (user == null)
        {
            Log.Warning("[CHANGE_PASSWORD] User not found for userId: [{UserId}]", userId);
            throw new UnauthorizedAccessException("User not found");
        }

        if (user.Email != request.Email)
        {
            Log.Warning("[CHANGE_PASSWORD] Unauthorized attempt to change password for email: [{Email}]",
                request.Email);
            throw new UnauthorizedAccessException("Unauthorized attempt to change password");
        }

        var changePasswordResult = await repository.ChangePasswordAsync(request);
        Log.Information("[CHANGE_PASSWORD] Password change [{Result}] for userId: [{UserId}]",
            changePasswordResult ? Succeeded : Failed, userId);
        return changePasswordResult;
    }

    public async Task LogoutServiceAsync()
    {
        Log.Information("[LOGOUT] Attempting to logout the current user.");

        await repository.LogoutAsync();
        Log.Information("[LOGOUT] User logged out successfully.");
    }

    public async Task<bool> ForgotPasswordServiceAsync(string email, string newPassword)
    {
        Log.Information("[FORGOT_PASSWORD] Attempting to process forgot password for email: [{Email}]", email);

        var result = await repository.ForgotPasswordAsync(email, newPassword);
        Log.Information("[FORGOT_PASSWORD] Forgot password process [{Result}] for email: [{Email}]",
            result ? Succeeded : Failed,
            email);
        return result;
    }

    public async Task<TokenDtoResponse> RefreshTokenServiceAsync(RefreshTokenDtoRequest refreshTokenDtoRequest)
    {
        Log.Information("[REFRESH_TOKEN] Attempting to refresh token.");

        try
        {
            var principal = ValidateRefreshToken(refreshTokenDtoRequest.RefreshToken!);

            var userEmail = principal.FindFirst(ClaimTypes.Email)?.Value!;
            var user = await GetUserByEmailAsync(userEmail);

            tokenManagerService.RevokeAllUserTokens(user);
            var tokenResponse = await tokenManagerService.GenerateTokenResponseAsync(user);

            Log.Information("[REFRESH_TOKEN] Token refreshed successfully for user: [{Email}]", userEmail);
            return tokenResponse;
        }
        catch (TokenRefreshException ex)
        {
            Log.Error(ex, "[REFRESH_TOKEN] Error processing token refresh request.");
            throw new TokenRefreshException("[REFRESH_TOKEN] Error processing token refresh request.", ex);
        }
    }

    public async Task<bool> RevokedTokenServiceAsync(string token)
    {
        Log.Information("[REVOKED_TOKEN] Attempting to revoke token.");
        var result = await tokenManagerService.RevokedTokenAsync(token);
        Log.Information("[REVOKED_TOKEN] Token revoke [{Result}]", result ? Succeeded : Failed);
        return result;
    }

    public async Task<bool> ExpiredTokenServiceAsync(string token)
    {
        Log.Information("[EXPIRED_TOKEN] Attempting to expire token.");
        var result = await tokenManagerService.ExpiredTokenAsync(token);
        Log.Information("[EXPIRED_TOKEN] Token expire [{Result}]", result ? Succeeded : Failed);
        return result;
    }

    private ClaimsPrincipal ValidateRefreshToken(string refreshToken)
    {
        var principal = tokenService.ValidateToken(refreshToken);
        if (principal != null) return principal;
        Log.Warning("[REFRESH_TOKEN] Invalid refresh token provided.");
        throw new UnauthorizedAccessException("Invalid refresh token");
    }

    private async Task<User> GetUserByEmailAsync(string email)
    {
        var user = await repository.GetUserProfileAsync(email);
        if (user != null) return user;
        Log.Warning("[REFRESH_TOKEN] User not found for email: [{Email}]", email);
        throw new UnauthorizedAccessException("User not found");
    }
}