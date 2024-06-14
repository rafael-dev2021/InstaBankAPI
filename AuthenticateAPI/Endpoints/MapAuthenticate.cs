using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticateAPI.Endpoints;

public static class MapAuthenticate
{
    public static void MapAuthenticateEndpoints(this WebApplication app)
    {
        MapPostEndpoint<LoginDtoRequest>(
            app,
            "/v1/auth/login",
            async (service, request) => await service.LoginAsync(request)
        );

        MapPostEndpoint<RegisterDtoRequest>(
            app,
            "/v1/auth/register",
            async (service, request) =>
            {
                var response = await service.RegisterAsync(request);
                var location = $"/v1/users/{request.Email}";
                return Results.Created(location, response);
            }
        );

        MapAuthorizedEndpoint<UpdateUserDtoRequest>(
            app,
            "/v1/auth/update-profile",
            async (service, request, userId) => await service.UpdateAsync(request, userId)
        );

        MapAuthorizedEndpoint<ChangePasswordDtoRequest>(
            app,
            "/v1/auth/change-password",
            async (service, request, _) => await service.ChangePasswordAsync(request)
        );
    }

    private static void MapPostEndpoint<T>(
        WebApplication app,
        string route,
        Func<AuthenticateService, T, Task<object>> handler) where T : class
    {
        app.MapPost(route, async (
            [FromServices] AuthenticateService service,
            [FromBody] T request,
            IValidator<T> validator) =>
        {
            var validationResult = await ValidateAsync(request, validator);
            if (validationResult != null)
            {
                return validationResult;
            }

            return await HandleRequestAsync(service, handler, request);
        });
    }

    private static void MapAuthorizedEndpoint<T>(
        WebApplication app,
        string route,
        Func<AuthenticateService, T, string, Task<object>> handler) where T : class
    {
        app.MapPut(route, [Authorize] async (
            [FromServices] AuthenticateService service,
            [FromBody] T request,
            [FromQuery] string userId,
            IValidator<T> validator) =>
        {
            var validationResult = await ValidateAsync(request, validator);
            if (validationResult != null)
            {
                return validationResult;
            }

            return await HandleAuthorizedRequestAsync(service, handler, request, userId);
        });
    }

    private static async Task<IResult?> ValidateAsync<T>(T request, IValidator<T> validator)
    {
        var result = await validator.ValidateAsync(request);
        return !result.IsValid ? Results.ValidationProblem(result.ToDictionary()) : null;
    }

    private static async Task<IResult> HandleRequestAsync<T>(
        AuthenticateService service,
        Func<AuthenticateService, T, Task<object>> handler,
        T request)
    {
        try
        {
            var response = await handler(service, request);
            return Results.Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Results.Json(new { ex.Message }, statusCode: 400);
        }
    }

    private static async Task<IResult> HandleAuthorizedRequestAsync<T>(
        AuthenticateService service,
        Func<AuthenticateService, T, string, Task<object>> handler,
        T request,
        string userId)
    {
        try
        {
            var response = await handler(service, request, userId);
            return Results.Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Results.Json(new { ex.Message }, statusCode: 400);
        }
    }
}