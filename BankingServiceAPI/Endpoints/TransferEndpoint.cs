using BankingServiceAPI.Dto.Request;
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
            "/api/transfer/by-account-number",
            async (service, request) => await service.TransferAsync(
                request.OriginAccountNumber,
                request.DestinationAccountNumber, 
                request.Amount),
            requireAuthentication: true
        );

        MapPostEndpoint<TransferDtoRequestByCpf>(
            app,
            "/api/transfer/by-cpf",
            async (service, request) => await service.TransferByCpfAsync(
                request.OriginCpf!, 
                request.DestinationCpf, 
                request.Amount),
            requireAuthentication: true
        );
    }
    
    public static void MapPostEndpoint<T>(
        WebApplication app,
        string route,
        Func<ITransferService, T, Task<object>> handler,
        int expectedStatusCode = StatusCodes.Status200OK,
        bool requireAuthentication = true) where T : class
    {
        app.MapPost(route, async (
            [FromServices] ITransferService service,
            [FromBody] T request,
            [FromServices] IValidator<T> validator,
            HttpContext context) =>
        {
            if (requireAuthentication && !context.User.Identity!.IsAuthenticated)
            {
                return Results.StatusCode(StatusCodes.Status401Unauthorized);
            }

            var validationResult = await ValidateAsync(request, validator);
            if (validationResult != null)
            {
                return validationResult;
            }

            return await HandleRequestAsync(service, handler, request, expectedStatusCode);
        }).RequireAuthorization();
    }

    public static async Task<IResult?> ValidateAsync<T>(T request, IValidator<T> validator)
    {
        var result = await validator.ValidateAsync(request);
        return !result.IsValid ? Results.ValidationProblem(result.ToDictionary()) : null;
    }

    public static async Task<IResult> HandleRequestAsync<T>(
        ITransferService service,
        Func<ITransferService, T, Task<object>> handler,
        T request,
        int expectedStatusCode)
    {
        try
        {
            var response = await handler(service, request);
            return expectedStatusCode switch
            {
                StatusCodes.Status201Created => Results.Created("", response),
                _ => Results.Ok(response)
            };
        }
        catch (UnauthorizedAccessException ex)
        {
            var errorResponse = new Dictionary<string, string> { { "Message", ex.Message } };
            return Results.Json(errorResponse, statusCode: StatusCodes.Status400BadRequest);
        }
    }
}