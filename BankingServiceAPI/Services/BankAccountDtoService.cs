using AutoMapper;
using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Models;
using BankingServiceAPI.Repositories.Interfaces;
using BankingServiceAPI.Services.Interfaces;
using Serilog;

namespace BankingServiceAPI.Services;

public class BankAccountDtoService(
    IBankAccountRepository repository,
    IMapper mapper,
    IUserContextService userContextService) : IBankAccountDtoService
{
    private const string Message = "An unexpected error occurred while processing the request.";

    public async Task<IEnumerable<BankAccountDtoResponse>> GetEntitiesDtoAsync()
    {
        try
        {
            var entities = await repository.GetEntitiesAsync();
            Log.Information("[GET_ENTITIES] Retrieved all bank accounts.");
            return !entities.Any() ? [] : mapper.Map<IEnumerable<BankAccountDtoResponse>>(entities);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[GET_ENTITIES] Error occurred while retrieving all bank accounts.");
            throw new BankAccountDtoServiceException(Message, ex);
        }
    }

    public async Task<BankAccountDtoResponse?> GetEntityDtoByIdAsync(int? id)
    {
        Log.Information("[GET_ENTITY_BY_ID] Fetching bank account with ID [{Id}].", id);
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

            Log.Information("[ADD_ENTITY] Created new bank account with ID [{Id}] for user [{UserId}].", bankAccount.Id,
                user.Id);
            return mapper.Map<BankAccountDtoResponse>(createdBankAccount);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[ADD_ENTITY] Error occurred while creating new bank account.");
            throw new BankAccountDtoServiceException("An unexpected error occurred while processing the request.", ex);
        }
    }

    public async Task DeleteEntityDtoAsync(int? id)
    {
        Log.Information("[DELETE_ENTITY] Deleting bank account with ID [{Id}].", id);
        var bankAccount = await GetBankAccountOrThrowAsync(id, "delete");

        await repository.DeleteEntityAsync(bankAccount.Id);
        Log.Information("[DELETE_ENTITY] Successfully deleted bank account with ID [{Id}].", id);
    }

    private async Task<BankAccount> GetBankAccountOrThrowAsync(int? id, string action)
    {
        var bankAccount = await repository.GetEntityByIdAsync(id);
        if (bankAccount != null) return bankAccount;

        Log.Warning("[GET_BANK_ACCOUNT] Bank account with ID [{Id}] not found for action [{Action}].", id, action);
        throw new GetIdNotFoundException($"Could not find bank account id: '{id}' ");
    }
}