using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticateAPI.Endpoints;
 
public static class MapAuthenticate
{
    public static void MapAuthenticateEndpoints(this WebApplication app)
    {
        MapPostEndpoint<LoginDtoRequest>(
            app,
            "/v1/auth/login",
            async (service, request) => await service.LoginAsync(request),
            requireAuthentication: false
        );

        MapPostEndpoint<RegisterDtoRequest>(
            app,
            "/v1/auth/register",
            async (service, request) =>
            {
                var response = await service.RegisterAsync(request);
                var location = $"/v1/auth/users/{request.Email}";
                return Results.Created(location, response);
            },
            StatusCodes.Status201Created,
            requireAuthentication: false
        );

        MapPostEndpoint<ForgotPasswordDtoRequest>(
            app,
            "/v1/auth/forgot-password",
            async (service, request) =>
            {
                var success = await service.ForgotPasswordAsync(request.Email!, request.NewPassword!);
                return Results.Ok(success);
            },
            requireAuthentication: false
        );

        MapPostEndpoint(
            app,
            "/v1/auth/logout",
            async service =>
            {
                await service.LogoutAsync();
                return null!;
            },
            expectedStatusCode: StatusCodes.Status204NoContent,
            requireAuthentication: true
        );

        MapAuthorizedEndpoint<UpdateUserDtoRequest>(
            app,
            "/v1/auth/update-profile",
            async (service, request, userId) => await service.UpdateAsync(request, userId),
            requireAuthentication: true
        );

        MapAuthorizedEndpoint<ChangePasswordDtoRequest>(
            app,
            "/v1/auth/change-password",
            async (service, request, _) => await service.ChangePasswordAsync(request),
            requireAuthentication: true
        );

        MapGetEndpoint<IEnumerable<UserDtoResponse>>(
            app,
            "/v1/auth/users",
            async service => await service.GetAllUsersDtoAsync(),
            requireAuthentication: true
        );
        
        MapPostEndpoint<RefreshTokenDtoRequest>(
            app,
            "/v1/auth/refresh-token",
            async (service, request) => await service.RefreshTokenAsync(request),
            requireAuthentication: false
        );
        
        MapPostEndpoint<string>(
            app,
            "/v1/auth/revoke-token",
            async (service, token) =>
            {
                var success = await service.RevokedTokenAsync(token);
                return Results.Ok(success);
            },
            requireAuthentication: true
        );
        
        MapPostEndpoint<string>(
            app,
            "/v1/auth/expired-token",
            async (service, token) =>
            {
                var success = await service.ExpiredTokenAsync(token);
                return Results.Ok(success);
            },
            requireAuthentication: true
        );
    }

    public static void MapGetEndpoint<T>(
        WebApplication app,
        string route,
        Func<IAuthenticateService, Task<T>> handler,
        bool requireAuthentication = true)
    {
        app.MapGet(route, async (
            [FromServices] IAuthenticateService service,
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

    public static void MapPostEndpoint<T>(
        WebApplication app,
        string route,
        Func<IAuthenticateService, T, Task<object>> handler,
        int expectedStatusCode = StatusCodes.Status200OK,
        bool requireAuthentication = true) where T : class
    {
        app.MapPost(route, async (
            [FromServices] IAuthenticateService service,
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
        });
    }

    public static void MapPostEndpoint(
        WebApplication app,
        string route,
        Func<IAuthenticateService, Task<object>> handler,
        int expectedStatusCode = StatusCodes.Status204NoContent,
        bool requireAuthentication = true)
    {
        app.MapPost(route, async (
            [FromServices] IAuthenticateService service,
            HttpContext context) =>
        {
            if (requireAuthentication && !context.User.Identity!.IsAuthenticated)
            {
                return Results.StatusCode(StatusCodes.Status401Unauthorized);
            }

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
        Func<IAuthenticateService, T, string, Task<object>> handler,
        bool requireAuthentication = true) where T : class
    {
        app.MapPut(route, async (
            [FromServices] IAuthenticateService service,
            [FromBody] T request,
            [FromQuery] string userId,
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