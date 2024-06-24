using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.Dto.Response;

namespace BankingServiceAPI.Services.Interfaces;

public interface IBankAccountDtoService
{
    Task<IEnumerable<BankAccountDtoResponse>> GetEntitiesDtoAsync();
    Task<BankAccountDtoResponse?> GetEntityDtoByIdAsync(int? id);
    Task AddEntityDtoAsync(BankAccountDtoRequest bankAccountDtoRequest, HttpContext httpContext);
    Task DeleteEntityDtoAsync(int? id);
}