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
                HttpContext httpContext) =>
            {
                var validator = httpContext.RequestServices.GetRequiredService<IValidator<LoginDtoRequest>>();
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                    return Results.BadRequest(errors);
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
            async (AuthenticateService service, RegisterDtoRequest request, HttpContext httpContext) =>
            {
                var validator = httpContext.RequestServices.GetRequiredService<IValidator<RegisterDtoRequest>>();
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                    return Results.BadRequest(errors);
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
                HttpContext httpContext) =>
            {
                var validator = httpContext.RequestServices.GetRequiredService<IValidator<UpdateUserDtoRequest>>();
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                    return Results.BadRequest(errors);
                }

                try
                {
                    var response = await service.UpdateProfileAsync(request, userId);
                    return Results.Ok(response);
                }
                catch (UnauthorizedAccessException ex)
                {
                    return Results.Json(new { ex.Message }, statusCode: 400);
                }
            });
    }
}