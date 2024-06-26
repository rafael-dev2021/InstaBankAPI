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
    private const string Message = "An unexpected error occurred while processing the request.";

    public async Task<IEnumerable<BankAccountDtoResponse>> GetEntitiesDtoAsync()
    {
        try
        {
            var entities = await repository.GetEntitiesAsync();
            logger.LogInformation("Returning all entities");
            return !entities.Any() ? [] : mapper.Map<IEnumerable<BankAccountDtoResponse>>(entities);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while getting all entities");
            throw new BankAccountDtoServiceException(Message, ex);
        }
    }

    public async Task<BankAccountDtoResponse?> GetEntityDtoByIdAsync(int? id)
    {
        logger.LogInformation("Getting entity by id: {Id}", id);
        var bankAccount = await GetBankAccountOrThrowAsync(id, "get");

        return mapper.Map<BankAccountDtoResponse>(bankAccount);
    }

    public async Task<BankAccountDtoResponse> AddEntityDtoAsync(BankAccountDtoRequest bankAccountDtoRequest,
        HttpContext httpContext)
    {
        try
        {
            var user = await userContextService.GetUserFromHttpContextAsync(httpContext);

            var bankAccount = mapper.Map<BankAccount>(bankAccountDtoRequest);
            bankAccount.SetUser(user);

            await repository.CreateEntityAsync(bankAccount);

            var createdBankAccount = await repository.GetEntityByIdAsync(bankAccount.Id);

            var bankAccountDtoResponse = mapper.Map<BankAccountDtoResponse>(createdBankAccount);

            return bankAccountDtoResponse;
        }
        catch (Exception ex)
        {
            throw new BankAccountDtoServiceException("An unexpected error occurred while processing the request.", ex);
        }
    }

    public async Task DeleteEntityDtoAsync(int? id)
    {
        logger.LogInformation("Deleting entity with id: {Id}", id);
        var bankAccount = await GetBankAccountOrThrowAsync(id, "delete");

        await repository.DeleteEntityAsync(bankAccount.Id);
        logger.LogInformation("Entity with id: {Id} deleted successfully", id);
    }

    private async Task<BankAccount> GetBankAccountOrThrowAsync(int? id, string action)
    {
        var bankAccount = await repository.GetEntityByIdAsync(id);
        if (bankAccount != null) return bankAccount;

        logger.LogWarning("Could not find bank account '{id}' for action {Action}", id, action);
        throw new GetIdNotFoundException($"Could not find bank account id: '{id}' ");
    }
}