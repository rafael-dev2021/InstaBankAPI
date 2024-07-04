using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.Dto.Response;
using BankingServiceAPI.Endpoints.Strategies;
using BankingServiceAPI.Exceptions;
using BankingServiceAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace BankingServiceAPI.Endpoints;

public static class BankAccountEndpoint
{
    public static void MapBankAccountEndpoints(this WebApplication app)
    {
        MapGetBankAccountsEndpoint<IEnumerable<BankAccountDtoResponse>>(
            app,
            "/v1/bank/accounts",
            async service => await service.GetEntitiesDtoAsync(),
            "cached_bank_accounts_list"
        );

        MapGetBankAccountByIdEndpoint<BankAccountDtoResponse>(
            app,
            "/v1/bank/find/{id:int}",
            async (service, id) => (await service.GetEntityDtoByIdAsync(id))!,
            "cached_bank_account_by_id"
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
            async (service, id) => await service.DeleteEntityDtoAsync(id)
        );
    }

    private static void MapGetBankAccountsEndpoint<T>(
        WebApplication app,
        string route,
        Func<IBankAccountDtoService, Task<T>> handler,
        string cacheKey)
    {
        app.MapGet(route, async (
            [FromServices] IBankAccountDtoService service,
            [FromServices] IDistributedCache cache,
            HttpContext context) =>
        {
            var authResult = AuthenticationRules.CheckAdminRole(context);
            if (authResult != null)
                return authResult;

            return await HandleCacheAsync(cache, cacheKey, () => handler(service));
        }).RequireAuthorization();
    }

    private static void MapGetBankAccountByIdEndpoint<T>(
        WebApplication app,
        string route,
        Func<IBankAccountDtoService, int?, Task<T>> handler,
        string cacheKey)
    {
        app.MapGet(route, async (
            [FromServices] IBankAccountDtoService service,
            [FromServices] IDistributedCache cache,
            int? id,
            HttpContext context) =>
        {
            try
            {
                var authResult = AuthenticationRules.CheckAdminRole(context);
                if (authResult != null)
                    return authResult;

                return await HandleCacheAsync(cache, cacheKey + id, () => handler(service, id));
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
            [FromServices] IDistributedCache cache,
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

                await cache.RemoveAsync("cached_bank_accounts_list");

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
        Func<IBankAccountDtoService, int?, Task> handler)
    {
        app.MapDelete(route, async (
            [FromServices] IBankAccountDtoService service,
            [FromServices] IDistributedCache cache,
            int? id,
            HttpContext context) =>
        {
            try
            {
                var authResult = AuthenticationRules.CheckAdminRole(context);
                if (authResult != null)
                {
                    return authResult;
                }

                await cache.RemoveAsync("cached_bank_accounts_list");
                await cache.RemoveAsync("cached_bank_account_by_id" + id);

                await handler(service, id);
                return Results.Ok();
            }
            catch (GetIdNotFoundException ex)
            {
                return Results.NotFound(new { message = ex.Message });
            }
        });
    }

    public static async Task<IResult> HandleCacheAsync<T>(
        IDistributedCache cache,
        string cacheKey,
        Func<Task<T>> fetchFromService)
    {
        var cachedData = await cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            var deserializedData = JsonConvert.DeserializeObject<T>(cachedData);
            return Results.Ok(deserializedData);
        }

        var result = await fetchFromService();
        var serializedData = JsonConvert.SerializeObject(result);

        await cache.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });

        return Results.Ok(result);
    }
}