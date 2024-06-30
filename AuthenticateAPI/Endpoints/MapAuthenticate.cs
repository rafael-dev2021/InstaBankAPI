using System.Security.Claims;
using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Endpoints.Strategies;
using AuthenticateAPI.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticateAPI.Endpoints;

public static class MapAuthenticate
{
    public static void MapAuthenticateEndpoints(this WebApplication app)
    {
        MapGetUsersEndpoint<IEnumerable<UserDtoResponse>>(
            app,
            "/v1/auth/users",
            async service => await service.GetAllUsersDtoAsync()
        );

        MapPostUnauthorizedEndpoint<LoginDtoRequest>(
            app,
            "/v1/auth/login",
            async (service, request) =>
            {
                var response = await service.LoginAsync(request);
                return Results.Ok(response);
            });

        MapPostUnauthorizedEndpoint<RegisterDtoRequest>(
            app,
            "/v1/auth/register",
            async (service, request) =>
            {
                var response = await service.RegisterAsync(request);
                return Results.Created($"/v1/auth/users/{request.Email}", response);
            }
        );

        MapPostUnauthorizedEndpoint<ForgotPasswordDtoRequest>(
            app,
            "/v1/auth/forgot-password",
            async (service, request) =>
            {
                var success = await service.ForgotPasswordAsync(request.Email!, request.NewPassword!);
                return Results.Ok(success);
            }
        );

        MapPostLogoutEndpoint(
            app,
            "/v1/auth/logout",
            async service =>
            {
                await service.LogoutAsync();
                return null!;
            },
            expectedStatusCode: StatusCodes.Status204NoContent
        );

        MapPutAuthorizedEndpoint<UpdateUserDtoRequest>(
            app,
            "/v1/auth/update-profile",
            async (service, request, userId) => await service.UpdateUserDtoAsync(request, userId)
        );

        MapPutAuthorizedEndpoint<ChangePasswordDtoRequest>(
            app,
            "/v1/auth/change-password",
            async (service, request, userId) => await service.ChangePasswordAsync(request, userId)
        );

        MapPostAuthorizeEndpoint<RefreshTokenDtoRequest>(
            app,
            "/v1/auth/refresh-token",
            async (service, request) =>
            {
                var success = await service.RefreshTokenAsync(request);
                return Results.Ok(success);
            });

        MapGetTokenValidationEndpoint(
            app,
            "/v1/auth/revoked-token",
            async
                (service, token) =>
            {
                var success = await service.RevokedTokenAsync(token);
                return Results.Ok(success);
            });

        MapGetTokenValidationEndpoint(
            app,
            "/v1/auth/expired-token",
            async
                (service, token) =>
            {
                var success = await service.ExpiredTokenAsync(token);
                return Results.Ok(success);
            });
    }

    private static void MapGetUsersEndpoint<T>(
        WebApplication app,
        string route,
        Func<IAuthenticateService, Task<T>> handler)
    {
        app.MapGet(route, async (
            [FromServices] IAuthenticateService service,
            HttpContext context) =>
        {
            try
            {
                var authResult =
                    AuthenticationRules.CheckAdminRole(context);
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
        }).RequireAuthorization();
    }

    private static void MapGetTokenValidationEndpoint(
        WebApplication app,
        string route,
        Func<IAuthenticateService, string, Task<IResult>> handler)
    {
        app.MapGet(route, async (
            [FromServices] IAuthenticateService service,
            [FromQuery] string token) =>
        {
            var result = await handler(service, token);
            return result;
        }).RequireAuthorization();
    }

    private static void MapPostUnauthorizedEndpoint<T>(
        WebApplication app,
        string route,
        Func<IAuthenticateService, T, Task<object>> handler) where T : class
    {
        app.MapPost(route, async (
            [FromServices] IAuthenticateService service,
            [FromBody] T request,
            [FromServices] IValidator<T> validator) =>
        {
            try
            {
                var validationResult = await RequestValidator.ValidateAsync(request, validator);
                if (validationResult != null)
                {
                    return validationResult;
                }

                var result = await handler(service, request);
                return result;
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
        });
    }

    private static void MapPostAuthorizeEndpoint<T>(
        WebApplication app,
        string route,
        Func<IAuthenticateService, T, Task<IResult>> handler) where T : class
    {
        app.MapPost(route, async (
            [FromServices] IAuthenticateService service,
            [FromBody] T request) =>
        {
            try
            {
                var result = await handler(service, request);
                return result;
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
        });
    }

    private static void MapPostLogoutEndpoint(
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
        }).RequireAuthorization();
    }

    private static void MapPutAuthorizedEndpoint<T>(
        WebApplication app,
        string route,
        Func<IAuthenticateService, T, string, Task<object>> handler) where T : class
    {
        app.MapPut(route, async (
            [FromServices] IAuthenticateService service,
            [FromBody] T request,
            HttpContext context,
            [FromServices] IValidator<T> validator) =>
        {
            try
            {
                var validationResult = await RequestValidator.ValidateAsync(request, validator);
                if (validationResult != null)
                {
                    return validationResult;
                }

                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Results.Unauthorized();
                }

                return await handler(service, request, userId);
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
        }).RequireAuthorization();
    }
}