using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.Endpoints.Strategies;
using BankingServiceAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BankingServiceAPI.Endpoints;

public static class DepositEndpoint
{
    public static void MapDepositEndpoint(this WebApplication app)
    {
        app.MapPost("/v1/bank/deposit", async (
            [FromServices] IDepositService service,
            [FromBody] DepositDtoRequest request,
            [FromServices] IValidator<DepositDtoRequest> validator,
            HttpContext context) =>
        {
            var (errorResult, userId) = await RequestHandler.HandleRequestAsync(context, request, validator);
            if (errorResult != null)
            {
                return errorResult;
            }

            return await RequestHandler.HandleServiceCallAsync(async () =>
            {
                var deposit = await service.DepositAsync(userId!, request.AccountNumber, request.Amount);
                return Results.Ok(deposit);
            });
        }).RequireAuthorization();
    }
}