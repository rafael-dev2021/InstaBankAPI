using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;

namespace AuthenticateAPI.Repositories.Strategies;

public interface IAuthenticatedStrategy
{
    Task<AuthenticatedDtoResponse> AuthenticatedAsync(LoginDtoRequest request);
}