using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.Endpoints.Strategies;
using BankingServiceAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BankingServiceAPI.Endpoints;

public static class WithdrawEndpoint
{
    public static void MapWithdrawEndpoint(this WebApplication app)
    {
        app.MapPost("/v1/bank/withdraw", async (
            [FromServices] IWithdrawDtoService service,
            [FromBody] WithdrawDtoRequest request,
            [FromServices] IValidator<WithdrawDtoRequest> validator,
            HttpContext context) =>
        {
            var (errorResult, userId) = await RequestHandler.HandleRequestAsync(context, request, validator);
            if (errorResult != null)
            {
                return errorResult;
            }

            return await RequestHandler.HandleServiceCallAsync(async () =>
            {
                var withdraw = await service.WithdrawDtoAsync(userId!, request.AccountNumber, request.Amount);
                return Results.Ok(withdraw);
            });
        }).RequireAuthorization();
    }
}
