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
        app.MapPost("/v1/auth/login",
            async (
                [FromServices] AuthenticateService service,
                [FromBody] LoginDtoRequest request,
                IValidator<LoginDtoRequest> validator) =>
            {
                var validationResult = await ValidateAsync(request, validator);
                if (validationResult != null)
                {
                    return validationResult;
                }

                try
                {
                    var response = await service.LoginAsync(request);
                    return Results.Ok(response);
                }
                catch (UnauthorizedAccessException ex)
                {
                    return Results.Json(new { ex.Message }, statusCode: 400);
                }
            });

        app.MapPost("/v1/auth/register",
            async (
                [FromServices] AuthenticateService service,
                [FromBody] RegisterDtoRequest request,
                IValidator<RegisterDtoRequest> validator) =>
            {
                var validationResult = await ValidateAsync(request, validator);
                if (validationResult != null)
                {
                    return validationResult;
                }

                try
                {
                    var response = await service.RegisterAsync(request);
                    var location = $"/v1/users/{request.Email}";
                    return Results.Created(location, response);
                }
                catch (UnauthorizedAccessException ex)
                {
                    return Results.Json(new { ex.Message }, statusCode: 400);
                }
            });

        app.MapPut("/v1/auth/update-profile",
            [Authorize] async (
                [FromServices] AuthenticateService service,
                [FromBody] UpdateUserDtoRequest request,
                [FromQuery] string userId,
                IValidator<UpdateUserDtoRequest> validator) =>
            {
                var validationResult = await ValidateAsync(request, validator);
                if (validationResult != null)
                {
                    return validationResult;
                }

                try
                {
                    var response = await service.UpdateAsync(request, userId);
                    return Results.Ok(response);
                }
                catch (UnauthorizedAccessException ex)
                {
                    return Results.Json(new { ex.Message }, statusCode: 400);
                }
            });

        app.MapPost("/v1/auth/change-password",
            [Authorize] async (
                [FromServices] AuthenticateService service,
                [FromBody] ChangePasswordDtoRequest request,
                IValidator<ChangePasswordDtoRequest> validator) =>
            {
                var validationResult = await ValidateAsync(request, validator);
                if (validationResult != null)
                {
                    return validationResult;
                }

                try
                {
                    var response = await service.ChangePasswordAsync(request);
                    return Results.Ok(response);
                }
                catch (UnauthorizedAccessException ex)
                {
                    return Results.Json(new { ex.Message }, statusCode: 400);
                }
            });
    }

    private static async Task<IResult?> ValidateAsync<T>(T request, IValidator<T> validator)
    {
        var result = await validator.ValidateAsync(request);
        return !result.IsValid ? Results.ValidationProblem(result.ToDictionary()) : null;
    }
}