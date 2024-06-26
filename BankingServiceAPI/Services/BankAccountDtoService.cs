using AutoMapper;
using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using BankingServiceAPI.Services.Interfaces;

namespace BankingServiceAPI.Services;

public class BankAccountDtoService(
    IBankAccountRepository repository,
    IMapper mapper,
    ILogger<BankAccountDtoService> logger,
    IUserContextService userContextService) : IBankAccountDtoService
{

    public async Task<IEnumerable<BankAccountDtoResponse>> GetEntitiesDtoAsync()
    {
        var entities = await repository.GetEntitiesAsync();
        logger.LogInformation("Returning all entities");
        return !entities.Any() ? [] : mapper.Map<IEnumerable<BankAccountDtoResponse>>(entities);
    }

    public async Task<BankAccountDtoResponse?> GetEntityDtoByIdAsync(int? id)
    {
        logger.LogInformation("Getting entity by id: {Id}", id);
        var getBankAccountId = await repository.GetEntityByIdAsync(id);

        if (getBankAccountId != null)
        {
            return mapper.Map<BankAccountDtoResponse>(getBankAccountId);
        }

        logger.LogWarning("Could not find bank account id: {Id}", id);
        throw new GetIdNotFoundException($"Could not find bank account id: {id}");
    }

    public async Task<BankAccountDtoResponse> AddEntityDtoAsync(BankAccountDtoRequest bankAccountDtoRequest,
        HttpContext httpContext)
    {
        var user = await userContextService.GetUserFromHttpContextAsync(httpContext);

        var bankAccount = mapper.Map<BankAccount>(bankAccountDtoRequest);
        bankAccount.SetUser(user);

        await repository.CreateEntityAsync(bankAccount);

        var createdBankAccount = await repository.GetEntityByIdAsync(bankAccount.Id);

        var bankAccountDtoResponse = mapper.Map<BankAccountDtoResponse>(createdBankAccount);

        return bankAccountDtoResponse;
    }

    public async Task DeleteEntityDtoAsync(int? id)
    {
        logger.LogInformation("Deleting entity with id: {Id}", id);
        var deleteBankAccountId = await repository.GetEntityByIdAsync(id);

        if (deleteBankAccountId == null)
        {
            logger.LogWarning("Could not find bank account id: {Id}", id);
            throw new GetIdNotFoundException($"Could not find bank account id: {id}");
        }

        await repository.DeleteEntityAsync(deleteBankAccountId.Id);
        logger.LogInformation("Entity with id: {Id} deleted successfully", id);
    }
}