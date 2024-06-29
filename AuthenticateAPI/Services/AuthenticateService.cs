using System.Security.Claims;
using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Repositories.Interfaces;
using AuthenticateAPI.Security;
using AutoMapper;

namespace AuthenticateAPI.Services;

public class AuthenticateService(
    IAuthenticatedRepository repository,
    ITokenService tokenService,
    ILogger<AuthenticateService> logger,
    IMapper mapper,
    ITokenManagerService tokenManagerService) : IAuthenticateService
{
    public async Task<IEnumerable<UserDtoResponse>> GetAllUsersDtoAsync()
    {
        var usersDto = await repository.GetAllUsersAsync();

        logger.LogInformation("Returning all users");

        return !usersDto.Any() ? [] : mapper.Map<IEnumerable<UserDtoResponse>>(usersDto);
    }

    public async Task<TokenDtoResponse> LoginAsync(LoginDtoRequest request)
    {
        logger.LogInformation("LoginAsync called with email: {Email}", request.Email);

        var authResponse = await repository.AuthenticateAsync(request);
        if (!authResponse.Success)
        {
            logger.LogWarning("Authentication failed for email: {Email} with message: {Message}", request.Email,
                authResponse.Message);
            throw new UnauthorizedAccessException(authResponse.Message);
        }

        var user = await repository.GetUserProfileAsync(request.Email!);

        tokenManagerService.RevokeAllUserTokens(user!);

        var tokenResponse = await tokenManagerService.GenerateTokenResponseAsync(user!);

        logger.LogInformation("Login successful for email: {Email}", request.Email);
        return tokenResponse;
    }

    public async Task<TokenDtoResponse> RegisterAsync(RegisterDtoRequest request)
    {
        logger.LogInformation("RegisterAsync called with email: {Email}", request.Email);
        var registerResponse = await repository.RegisterAsync(request);
        if (!registerResponse.Success)
        {
            logger.LogWarning("Registration failed for email: {Email} with message: {Message}", request.Email,
                registerResponse.Message);
            throw new UnauthorizedAccessException(registerResponse.Message);
        }

        var user = await repository.GetUserProfileAsync(request.Email!);

        var tokenResponse = await tokenManagerService.GenerateTokenResponseAsync(user!);

        logger.LogInformation("Registration successful for email: {Email}", request.Email);
        return tokenResponse;
    }

    public async Task<TokenDtoResponse> UpdateAsync(UpdateUserDtoRequest request, string userId)
    {
        logger.LogInformation("UpdateAsync called for userId: {UserId}", userId);

        var updateResponse = await repository.UpdateProfileAsync(request, userId);
        if (!updateResponse.Success)
        {
            logger.LogWarning("Profile update failed for userId: {UserId} with message: {Message}", userId,
                updateResponse.Message);
            throw new UnauthorizedAccessException(updateResponse.Message);
        }

        var user = await repository.GetUserProfileAsync(request.Email!);

        tokenManagerService.RevokeAllUserTokens(user!);

        var tokenResponse = await tokenManagerService.GenerateTokenResponseAsync(user!);

        logger.LogInformation("Profile update successful for userId: {UserId}", userId);
        return tokenResponse;
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordDtoRequest request)
    {
        logger.LogInformation("ChangePasswordAsync called for email: {Email}", request.Email);

        var result = await repository.ChangePasswordAsync(request);
        if (result)
        {
            logger.LogInformation("Password change successful for email: {Email}", request.Email);
        }
        else
        {
            logger.LogWarning("Password change failed for email: {Email}", request.Email);
        }

        return result;
    }

    public Task LogoutAsync()
    {
        logger.LogInformation("LogoutAsync called");

        return repository.LogoutAsync();
    }

    public async Task<bool> ForgotPasswordAsync(string email, string newPassword)
    {
        logger.LogInformation("ForgotPasswordAsync called for email: {Email}", email);

        return await repository.ForgotPasswordAsync(email, newPassword);
    }

    public async Task<TokenDtoResponse> RefreshTokenAsync(RefreshTokenDtoRequest refreshTokenDtoRequest)
    {
        logger.LogInformation("RefreshTokenAsync called");

        try
        {
            var principal = tokenService.ValidateToken(refreshTokenDtoRequest.RefreshToken!);

            if (principal == null)
            {
                logger.LogWarning("Invalid refresh token provided");
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            var userEmail = principal.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
            {
                logger.LogWarning("Email claim not found in token");
                throw new UnauthorizedAccessException("Invalid token");
            }

            var user = await repository.GetUserProfileAsync(userEmail);

            if (user == null)
            {
                logger.LogWarning("User not found for email: {Email}", userEmail);
                throw new UnauthorizedAccessException("User not found");
            }

            tokenManagerService.RevokeAllUserTokens(user);

            var tokenResponse = await tokenManagerService.GenerateTokenResponseAsync(user);

            logger.LogInformation("Token refreshed successfully for user: {Email}", userEmail);
            return tokenResponse;
        }
        catch (Exception ex)
        {
            logger.LogError("Error processing token refresh request: {Message}", ex.Message);
            throw new Exception("Internal server error while processing token refresh request", ex);
        }
    }

    public async Task<bool> RevokedTokenAsync(string token) =>
        await tokenManagerService.RevokedTokenAsync(token);

    public async Task<bool> ExpiredTokenAsync(string token) =>
        await tokenManagerService.ExpiredTokenAsync(token);
}