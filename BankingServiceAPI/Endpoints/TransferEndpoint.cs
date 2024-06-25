using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.Endpoints.Strategies;
using BankingServiceAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BankingServiceAPI.Endpoints;

public static class TransferEndpoint
{
    public static void MapTransferEndpoint(this WebApplication app)
    {
        MapPostEndpoint<TransferDtoRequestByAccount>(
            app,
            "/v1/transfer/by-account-number",
            async (service, userId, request) => await service.TransferAsync(
                userId,
                request.OriginAccountNumber,
                request.DestinationAccountNumber,
                request.Amount)
        );

        MapPostEndpoint<TransferDtoRequestByCpf>(
            app,
            "/v1/transfer/by-cpf",
            async (service, userId, request) => await service.TransferByCpfAsync(
                userId,
                request.OriginCpf!,
                request.DestinationCpf,
                request.Amount)
        );
    }

    public static void MapPostEndpoint<T>(
        WebApplication app,
        string route,
        Func<ITransferService, string, T, Task<object>> handler,
        int expectedStatusCode = StatusCodes.Status200OK) where T : class
    {
        app.MapPost(route, async (
            [FromServices] ITransferService service,
            [FromBody] T request,
            [FromServices] IValidator<T> validator,
            HttpContext context) =>
        {
            var (errorResult, userId) = await RequestHandler.HandleRequestAsync(context, request, validator);
            if (errorResult != null)
            {
                return errorResult;
            }

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
