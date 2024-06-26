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
        try
        {
            logger.LogInformation("Getting entity by id: {Id}", id);
            var getBankAccountId = await repository.GetEntityByIdAsync(id);

            if (getBankAccountId != null) return mapper.Map<BankAccountDtoResponse>(getBankAccountId);
            logger.LogWarning("Entity with id: {Id} not found", id);
            throw new GetIdNotFoundException($"Could not find bank account id: {id}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while getting entity by id: {Id}", id);
            throw new BankAccountDtoServiceException(Message, ex);
        }
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

            return new BankAccountDtoResponse(
                bankAccount.Id,
                bankAccount.AccountNumber,
                bankAccount.Balance,
                bankAccount.Agency,
                bankAccount.AccountType.ToString(),
                new UserDtoResponse(
                    user.Id,
                    user.Name,
                    user.LastName,
                    user.Cpf,
                    user.Email,
                    user.PhoneNumber,
                    user.Role
                )
            );
        }
        catch (Exception ex)
        {
            throw new BankAccountDtoServiceException(Message, ex);
        }
    }

    public async Task DeleteEntityDtoAsync(int? id)
    {
        try
        {
            logger.LogInformation("Deleting entity with id: {Id}", id);
            var deleteBankAccountId = await repository.GetEntityByIdAsync(id);

            if (deleteBankAccountId == null)
            {
                logger.LogWarning("Entity with id: {Id} not found for deletion", id);
                throw new GetIdNotFoundException($"Could not find bank account id: {id}");
            }

            await repository.DeleteEntityAsync(deleteBankAccountId.Id);
            logger.LogInformation("Entity with id: {Id} deleted successfully", id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while deleting entity with id: {Id}", id);
            throw new BankAccountDtoServiceException(Message, ex);
        }
    }
}