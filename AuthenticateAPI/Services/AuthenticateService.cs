using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Repositories;

namespace AuthenticateAPI.Services;

public class AuthenticateService(IAuthenticatedRepository repository, ITokenService tokenService)
{
    public async Task<TokenDtoResponse> LoginAsync(LoginDtoRequest request)
    {
        var authResponse = await repository.AuthenticateAsync(request);
        if (!authResponse.Success)
        {
            throw new UnauthorizedAccessException(authResponse.Message);
        }

        var user = await repository.GetUserProfileAsync(request.Email!);
        var token = tokenService.GenerateAccessToken(user!);
        var refreshToken = tokenService.GenerateRefreshToken(user!);

        return new TokenDtoResponse(token, refreshToken);
    }

    public async Task<TokenDtoResponse> RegisterAsync(RegisterDtoRequest request)
    {
        var register = await repository.RegisterAsync(request);
        if (!register.Success)
        {
            throw new UnauthorizedAccessException(register.Message);
        }

        var user = await repository.GetUserProfileAsync(request.Email!);
        var token = tokenService.GenerateAccessToken(user!);
        var refreshToken = tokenService.GenerateRefreshToken(user!);

        return new TokenDtoResponse(token, refreshToken);
    }

    public async Task<TokenDtoResponse> UpdateAsync(UpdateUserDtoRequest request, string userId)
    {
        var update = await repository.UpdateProfileAsync(request, userId);
        if (!update.Success)
        {
            throw new UnauthorizedAccessException(update.Message);
        }

        var user = await repository.GetUserProfileAsync(request.Email!);
        var token = tokenService.GenerateAccessToken(user!);
        var refreshToken = tokenService.GenerateRefreshToken(user!);

        return new TokenDtoResponse(token, refreshToken);
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordDtoRequest request)
    {
        return await repository.ChangePasswordAsync(request);
    }
}