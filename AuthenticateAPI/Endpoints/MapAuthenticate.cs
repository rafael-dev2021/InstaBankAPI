using System.Security.Claims;
using AuthenticateAPI.Dto.Request;
using AuthenticateAPI.Dto.Response;
using AuthenticateAPI.Endpoints.Strategies;
using AuthenticateAPI.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace AuthenticateAPI.Endpoints;

public static class MapAuthenticate
{
    public static void MapAuthenticateEndpoints(this WebApplication app)
    {
        MapGetUsersEndpoint<IEnumerable<UserDtoResponse>>(
            app,
            "/v1/auth/users",
            async service => await service.GetAllUsersServiceAsync()
        );

        MapPostUnauthorizedEndpoint<LoginDtoRequest>(
            app,
            "/v1/auth/login",
            async (service, request) =>
            {
                var response = await service.LoginServiceAsync(request);
                return Results.Ok(response);
            });

        MapPostUnauthorizedEndpoint<RegisterDtoRequest>(
            app,
            "/v1/auth/register",
            async (service, request) =>
            {
                var response = await service.RegisterServiceAsync(request);
                return Results.Created($"/v1/auth/users/{request.Email}", response);
            }
        );

        MapPostUnauthorizedEndpoint<ForgotPasswordDtoRequest>(
            app,
            "/v1/auth/forgot-password",
            async (service, request) =>
            {
                var success = await service.ForgotPasswordServiceAsync(request.Email!, request.NewPassword!);
                return Results.Ok(success);
            }
        );

        MapPostLogoutEndpoint(
            app,
            "/v1/auth/logout",
            async service =>
            {
                await service.LogoutServiceAsync();
                return null!;
            },
            expectedStatusCode: StatusCodes.Status204NoContent
        );

        MapPutAuthorizedEndpoint<UpdateUserDtoRequest>(
            app,
            "/v1/auth/update-profile",
            async (service, request, userId) => await service.UpdateUserServiceAsync(request, userId)
        );

        MapPutAuthorizedEndpoint<ChangePasswordDtoRequest>(
            app,
            "/v1/auth/change-password",
            async (service, request, userId) => await service.ChangePasswordServiceAsync(request, userId)
        );

        MapPostAuthorizeEndpoint<RefreshTokenDtoRequest>(
            app,
            "/v1/auth/refresh-token",
            async (service, request) =>
            {
                var success = await service.RefreshTokenServiceAsync(request);
                return Results.Ok(success);
            });

        MapGetTokenValidationEndpoint(
            app,
            "/v1/auth/revoked-token",
            async (service, token) =>
            {
                var success = await service.RevokedTokenServiceAsync(token);
                return success;
            },
            "cached_revoked"
        );

        MapGetTokenValidationEndpoint(
            app,
            "/v1/auth/expired-token",
            async (service, token) =>
            {
                var success = await service.ExpiredTokenServiceAsync(token);
                return success;
            },
            "cached_expired"
        );
    }

    private static void MapGetUsersEndpoint<T>(
        WebApplication app,
        string route,
        Func<IAuthenticateService, Task<T>> handler)
    {
        app.MapGet(route, async (
            [FromServices] IAuthenticateService service,
            [FromServices] IDistributedCache cache,
            HttpContext context) =>
        {
            try
            {
                var authResult = AuthenticationRules.CheckAdminRole(context);
                if (authResult != null)
                {
                    return authResult;
                }

                const string cacheKey = "cached_users";

                var cachedUsers = await cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedUsers))
                {
                    var usersDeserializers = JsonConvert.DeserializeObject<IEnumerable<UserDtoResponse>>(cachedUsers);
                    return Results.Ok(usersDeserializers);
                }

                var usersDto = await handler(service);
                var serializedUsers = JsonConvert.SerializeObject(usersDto);

                await cache.SetStringAsync(cacheKey, serializedUsers, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

                return Results.Ok(usersDto);
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
        Func<IAuthenticateService, string, Task<bool>> handler,
        string cacheKeyPrefix)
    {
        app.MapGet(route, async (
            [FromServices] IAuthenticateService service,
            [FromServices] IDistributedCache cache,
            [FromQuery] string token) =>
        {
            var cacheKey = $"{cacheKeyPrefix}_{token}";

            var cachedResult = await cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedResult))
            {
                var resultCached = JsonConvert.DeserializeObject<ApiTokensDtoResponse>(cachedResult);
                return Results.Ok(resultCached!.Success);
            }

            var success = await handler(service, token);
            var apiTokensDtoResponse = new ApiTokensDtoResponse(success);

            var serializedResult = JsonConvert.SerializeObject(apiTokensDtoResponse);
            await cache.SetStringAsync(cacheKey, serializedResult, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            return Results.Ok(success);
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
            [FromServices] IValidator<T> validator,
            [FromServices] IDistributedCache cache) =>
        {
            try
            {
                var validationResult = await RequestValidator.ValidateAsync(request, validator);
                if (validationResult != null)
                {
                    return validationResult;
                }

                await cache.RemoveAsync("cached_users");

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
            [FromServices] IDistributedCache cache,
            [FromBody] T request) =>
        {
            try
            {
                await cache.RemoveAsync("cached_users");
                
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
            [FromServices] IDistributedCache cache,
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
                await cache.RemoveAsync("cached_users");
                
                return await handler(service, request, userId);
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
        }).RequireAuthorization();
    }
}