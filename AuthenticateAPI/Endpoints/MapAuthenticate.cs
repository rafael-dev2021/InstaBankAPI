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
            },
            StatusCodes.Status201Created
        );
        
        MapPostEndpoint<ForgotPasswordDtoRequest>(
            app,
            "/v1/auth/forgot-password",
            async (service, request) =>
            {
                var success = await service.ForgotPasswordAsync(request.Email!, request.NewPassword!);
                return Results.Ok(success);
            }
        );

        MapPostEndpoint(
            app,
            "/v1/auth/logout",
            async service =>
            {
                await service.LogoutAsync();
                return Results.NoContent();
            });

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

        MapGetEndpoint(
            app,
            "/v1/auth/users",
            async service => await service.GetAllUsersDtoAsync());
    }

    public static void MapGetEndpoint(
        WebApplication app,
        string route,
        Func<IAuthenticateService, Task<object>> handler)
    {
        app.MapGet(route, [Authorize("Admin")] async (
            [FromServices] IAuthenticateService service) => await handler(service));
    }

    public static void MapPostEndpoint<T>(
        WebApplication app,
        string route,
        Func<IAuthenticateService, T, Task<object>> handler,
        int expectedStatusCode = StatusCodes.Status200OK) where T : class
    {
        app.MapPost(route, async (
            [FromServices] IAuthenticateService service,
            [FromBody] T request,
            IValidator<T> validator) =>
        {
            var validationResult = await ValidateAsync(request, validator);
            if (validationResult != null)
            {
                return validationResult;
            }

            return await HandleRequestAsync(service, handler, request, expectedStatusCode);
        });
    }

    public static void MapPostEndpoint(
        WebApplication app,
        string route,
        Func<IAuthenticateService, Task<object>> handler,
        int expectedStatusCode = StatusCodes.Status204NoContent)
    {
        app.MapPost(route, async (
            [FromServices] IAuthenticateService service) =>
        {
            try
            {
                await handler(service);
                return Results.StatusCode(expectedStatusCode);
            }
            catch (UnauthorizedAccessException ex)
            {
                var errorResponse = new Dictionary<string, string> { { "Message", ex.Message } };
                return Results.Json(errorResponse, statusCode: StatusCodes.Status400BadRequest);
            }
        });
    }

    public static void MapAuthorizedEndpoint<T>(
        WebApplication app,
        string route,
        Func<IAuthenticateService, T, string, Task<object>> handler) where T : class
    {
        app.MapPut(route, [Authorize] async (
            [FromServices] IAuthenticateService service,
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

    public static async Task<IResult?> ValidateAsync<T>(T request, IValidator<T> validator)
    {
        var result = await validator.ValidateAsync(request);
        return !result.IsValid ? Results.ValidationProblem(result.ToDictionary()) : null;
    }

    public static async Task<IResult> HandleRequestAsync<T>(
        IAuthenticateService service,
        Func<IAuthenticateService, T, Task<object>> handler,
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

    public static async Task<IResult> HandleAuthorizedRequestAsync<T>(
        IAuthenticateService service,
        Func<IAuthenticateService, T, string, Task<object>> handler,
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
            var errorResponse = new Dictionary<string, string> { { "Message", ex.Message } };
            return Results.Json(errorResponse, statusCode: StatusCodes.Status400BadRequest);
        }
    }
}