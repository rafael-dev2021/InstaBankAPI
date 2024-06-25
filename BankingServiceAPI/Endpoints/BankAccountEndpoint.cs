﻿using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.Dto.Response;
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
            async (service, request, context) => await service.AddEntityDtoAsync(request, context),
            StatusCodes.Status201Created,
            requireAuthentication: true
        );

        MapDeleteEndpoint(
            app,
            "/v1/bank/delete/{id:int}",
            async (service, id) => await service.DeleteEntityDtoAsync(id),
            requireAuthentication: true
        );
    }

    public static void MapGetEndpoint<T>(
        WebApplication app,
        string route,
        Func<IBankAccountDtoService, Task<T>> handler,
        bool requireAuthentication = true)
    {
        app.MapGet(route, async (
            [FromServices] IBankAccountDtoService service,
            HttpContext context) =>
        {
            if (requireAuthentication)
            {
                if (!context.User.Identity!.IsAuthenticated)
                {
                    return Results.StatusCode(StatusCodes.Status401Unauthorized);
                }

                if (!context.User.IsInRole("Admin"))
                {
                    return Results.StatusCode(StatusCodes.Status403Forbidden);
                }
            }

            var result = await handler(service);
            return Results.Ok(result);
        });
    }

    public static void MapGetEndpoint<T>(
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
            if (requireAuthentication)
            {
                if (!context.User.Identity!.IsAuthenticated)
                {
                    return Results.StatusCode(StatusCodes.Status401Unauthorized);
                }

                if (!context.User.IsInRole("Admin"))
                {
                    return Results.StatusCode(StatusCodes.Status403Forbidden);
                }
            }

            var result = await handler(service, id);
            return Results.Ok(result);
        });
    }

    public static void MapPostEndpoint<T>(
        WebApplication app,
        string route,
        Func<IBankAccountDtoService, T, HttpContext, Task> handler,
        int expectedStatusCode = StatusCodes.Status200OK,
        bool requireAuthentication = true) where T : class
    {
        app.MapPost(route, async (
            [FromServices] IBankAccountDtoService service,
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

            await handler(service, request, context);
            return Results.StatusCode(expectedStatusCode);
        });
    }

    public static void MapDeleteEndpoint(
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
            if (requireAuthentication)
            {
                if (!context.User.Identity!.IsAuthenticated)
                {
                    return Results.StatusCode(StatusCodes.Status401Unauthorized);
                }

                if (!context.User.IsInRole("Admin"))
                {
                    return Results.StatusCode(StatusCodes.Status403Forbidden);
                }
            }

            await handler(service, id);
            return Results.Ok();
        });
    }

    public static async Task<IResult?> ValidateAsync<T>(T request, IValidator<T> validator)
    {
        var result = await validator.ValidateAsync(request);
        return !result.IsValid ? Results.ValidationProblem(result.ToDictionary()) : null;
    }
}