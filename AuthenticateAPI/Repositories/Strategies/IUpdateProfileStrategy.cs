using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;

namespace AuthenticateAPI.Repositories.Strategies;

public interface IUpdateProfileStrategy
{
    Task<List<string>> ValidateAsync(string? email, string? phoneNumber, string userId);
    Task<UpdatedDtoResponse> UpdateProfileAsync(UpdateUserDtoRequest updateUserDtoRequest, string userId);
}