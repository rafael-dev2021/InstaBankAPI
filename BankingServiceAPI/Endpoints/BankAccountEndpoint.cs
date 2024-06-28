using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Endpoints.Strategies;
using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BankingServiceAPI.Endpoints;

public static class BankAccountEndpoint
{
    public static void MapBankAccountEndpoints(this WebApplication app)
    {
        MapGetEndpoint<IEnumerable<BankAccountDtoResponse>>(
            app,
            "/v1/bank/accounts",
            async service => await service.GetEntitiesDtoAsync(),
            requireAuthentication: true
        );

        MapGetEndpoint<BankAccountDtoResponse>(
            app,
            "/v1/bank/find/{id:int}",
            async (service, id) => (await service.GetEntityDtoByIdAsync(id))!,
            requireAuthentication: true
        );

        MapPostEndpoint<BankAccountDtoRequest>(
            app,
            "/v1/bank/create",
            async (service, request, context) =>
            {
                var response = await service.AddEntityDtoAsync(request, context);
                return Results.Created($"/v1/bank/accounts/{response.Id}", response);
            },
            requireAuthentication: true
        );

        MapDeleteEndpoint(
            app,
            "/v1/bank/delete/{id:int}",
            async (service, id) => await service.DeleteEntityDtoAsync(id),
            requireAuthentication: true
        );
    }

    private static void MapGetEndpoint<T>(
        WebApplication app,
        string route,
        Func<IBankAccountDtoService, Task<T>> handler,
        bool requireAuthentication = true)
    {
        app.MapGet(route, async (
            [FromServices] IBankAccountDtoService service,
            HttpContext context) =>
        {
            try
            {
                var authResult =
                    AuthenticationRules.CheckAuthenticationAndAuthorization(context, requireAuthentication);
                if (authResult != null)
                {
                    return authResult;
                }

                var result = await handler(service);
                return Results.Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
        });
    }

    private static void MapGetEndpoint<T>(
        WebApplication app,
        string route,
        Func<IBankAccountDtoService, int?, Task<T>> handler,
        bool requireAuthentication = true)
    {
        app.MapGet(route, async (
            [FromServices] IBankAccountDtoService service,
            int? id,
            HttpContext context) =>
        {
            try
            {
                var authResult =
                    AuthenticationRules.CheckAuthenticationAndAuthorization(context, requireAuthentication);
                if (authResult != null)
                {
                    return authResult;
                }

                var result = await handler(service, id);
                return Results.Ok(result);
            }
            catch (GetIdNotFoundException ex)
            {
                return Results.NotFound(new { message = ex.Message });
            }
        });
    }

    private static void MapPostEndpoint<T>(
        WebApplication app,
        string route,
        Func<IBankAccountDtoService, T, HttpContext, Task<IResult>> handler,
        bool requireAuthentication = true) where T : class
    {
        app.MapPost(route, async (
            [FromServices] IBankAccountDtoService service,
            [FromBody] T request,
            [FromServices] IValidator<T> validator,
            HttpContext context) =>
        {
            try
            {
                if (requireAuthentication && !context.User.Identity!.IsAuthenticated)
                {
                    return Results.StatusCode(StatusCodes.Status401Unauthorized);
                }

                var validationResult = await RequestValidator.ValidateAsync(request, validator);
                if (validationResult != null)
                {
                    return validationResult;
                }

                var result = await handler(service, request, context);
                return result;
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
        });
    }

    private static void MapDeleteEndpoint(
        WebApplication app,
        string route,
        Func<IBankAccountDtoService, int?, Task> handler,
        bool requireAuthentication = true)
    {
        app.MapDelete(route, async (
            [FromServices] IBankAccountDtoService service,
            int? id,
            HttpContext context) =>
        {
            try
            {
                var authResult =
                    AuthenticationRules.CheckAuthenticationAndAuthorization(context, requireAuthentication);
                if (authResult != null)
                {
                    return authResult;
                }

                await handler(service, id);
                return Results.Ok();
            }
            catch (GetIdNotFoundException ex)
            {
                return Results.NotFound(new { message = ex.Message });
            }
        });
    }
}