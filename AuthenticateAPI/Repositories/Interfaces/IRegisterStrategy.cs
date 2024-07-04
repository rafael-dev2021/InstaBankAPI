using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;

namespace AuthenticateAPI.Repositories.Interfaces;

public interface IRegisterStrategy
{
    Task<RegisteredDtoResponse> CreateUserAsync(RegisterDtoRequest request);
}