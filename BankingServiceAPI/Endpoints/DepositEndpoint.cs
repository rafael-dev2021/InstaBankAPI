using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.Endpoints.Strategies;
using BankingServiceAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace BankingServiceAPI.Endpoints;

public static class DepositEndpoint
{
    public static void MapDepositEndpoint(this WebApplication app)
    {
        app.MapPost("/v1/bank/deposit", async (
            [FromServices] IDepositDtoService service,
            [FromBody] DepositDtoRequest request,
            [FromServices] IValidator<DepositDtoRequest> validator,
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
                var deposit = await service.DepositDtoAsync(userId!, request.AccountNumber, request.Amount);
                return Results.Ok(deposit);
            });
        }).RequireAuthorization();
    }
}