using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;

namespace AuthenticateAPI.Repositories.Interfaces;

public interface IAuthenticatedStrategy
{
    Task<AuthenticatedDtoResponse> AuthenticatedAsync(LoginDtoRequest request);
}