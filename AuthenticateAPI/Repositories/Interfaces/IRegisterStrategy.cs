using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;

namespace AuthenticateAPI.Repositories.Interfaces;

public interface IRegisterStrategy
{
    Task<List<string>> ValidateAsync(string? cpf, string? email, string? phoneNumber);
    Task<RegisteredDtoResponse> CreateUserAsync(RegisterDtoRequest request);
}