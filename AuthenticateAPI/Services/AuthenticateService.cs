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
    IMapper mapper) : IAuthenticateService
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
            logger.LogWarning("Authentication failed for email: {Email} with message: {Message}", request.Email, authResponse.Message);
            throw new UnauthorizedAccessException(authResponse.Message);
        }

        var user = await repository.GetUserProfileAsync(request.Email!);
        var token = tokenService.GenerateAccessToken(user!);
        var refreshToken = tokenService.GenerateRefreshToken(user!);

        logger.LogInformation("Login successful for email: {Email}", request.Email);
        return new TokenDtoResponse(token, refreshToken);
    }

    public async Task<TokenDtoResponse> RegisterAsync(RegisterDtoRequest request)
    {
        logger.LogInformation("RegisterAsync called with email: {Email}", request.Email);
        var registerResponse = await repository.RegisterAsync(request);
        if (!registerResponse.Success)
        {
            logger.LogWarning("Registration failed for email: {Email} with message: {Message}", request.Email, registerResponse.Message);
            throw new UnauthorizedAccessException(registerResponse.Message);
        }

        var user = await repository.GetUserProfileAsync(request.Email!);
        var token = tokenService.GenerateAccessToken(user!);
        var refreshToken = tokenService.GenerateRefreshToken(user!);

        logger.LogInformation("Registration successful for email: {Email}", request.Email);
        return new TokenDtoResponse(token, refreshToken);
    }

    public async Task<TokenDtoResponse> UpdateAsync(UpdateUserDtoRequest request, string userId)
    {
        logger.LogInformation("UpdateAsync called for userId: {UserId}", userId);

        var updateResponse = await repository.UpdateProfileAsync(request, userId);
        if (!updateResponse.Success)
        {
            logger.LogWarning("Profile update failed for userId: {UserId} with message: {Message}", userId, updateResponse.Message);
            throw new UnauthorizedAccessException(updateResponse.Message);
        }

        var user = await repository.GetUserProfileAsync(request.Email!);
        var token = tokenService.GenerateAccessToken(user!);
        var refreshToken = tokenService.GenerateRefreshToken(user!);

        logger.LogInformation("Profile update successful for userId: {UserId}", userId);
        return new TokenDtoResponse(token, refreshToken);
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
}