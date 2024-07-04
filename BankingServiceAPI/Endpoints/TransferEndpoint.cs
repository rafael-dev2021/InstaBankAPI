using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.Endpoints.Strategies;
using BankingServiceAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace BankingServiceAPI.Endpoints;

public static class TransferEndpoint
{
    public static void MapTransferEndpoint(this WebApplication app)
    {
        MapPostEndpoint<TransferDtoRequestByAccount>(
            app,
            "/v1/transfer/by-account-number",
            async (service, userId, request) => await service.TransferByBankAccountNumberDtoAsync(
                userId,
                request.OriginAccountNumber,
                request.DestinationAccountNumber,
                request.Amount)
        );

        MapPostEndpoint<TransferDtoRequestByCpf>(
            app,
            "/v1/transfer/by-cpf",
            async (service, userId, request) => await service.TransferByCpfDtoAsync(
                userId,
                request.OriginCpf!,
                request.DestinationCpf,
                request.Amount)
        );
    }

    private static void MapPostEndpoint<T>(
        WebApplication app,
        string route,
        Func<ITransferDtoService, string, T, Task<object>> handler,
        int expectedStatusCode = StatusCodes.Status200OK) where T : class
    {
        app.MapPost(route, async (
            [FromServices] ITransferDtoService service,
            [FromBody] T request,
            [FromServices] IValidator<T> validator,
            [FromServices] IDistributedCache cache,
            HttpContext context) =>
        {
            var (errorResult, userId) = await RequestHandler.HandleRequestAsync(context, request, validator);
            if (errorResult != null)
            {
                return errorResult;
            }
            
            await cache.RemoveAsync("cached_bank_accounts_list");

            return await RequestHandler.HandleServiceCallAsync(async () =>
            {
                var response = await handler(service, userId!, request);
                return expectedStatusCode switch
                {
                    StatusCodes.Status201Created => Results.Created("", response),
                    _ => Results.Ok(response)
                };
            });
        }).RequireAuthorization();
    }
}
