using System.Security.Claims;
using FluentValidation;

namespace BankingServiceAPI.Endpoints.Strategies;

public static  class RequestHandler
{
    public static async Task<(IResult?, string?)> HandleRequestAsync<T>(
        HttpContext context, 
        T request, 
        IValidator<T> validator)
    {
        if (!context.User.Identity!.IsAuthenticated)
        {
            return (Results.StatusCode(StatusCodes.Status401Unauthorized), null);
        }

        var validationResult = await RequestValidator.ValidateAsync(request, validator);
        if (validationResult != null)
        {
            return (validationResult, null);
        }

        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return (Results.StatusCode(StatusCodes.Status401Unauthorized), null);
        }

        return (null, userId);
    }
    
    public static async Task<IResult> HandleServiceCallAsync(Func<Task<IResult>> serviceCall)
    {
        try
        {
            return await serviceCall();
        }
        catch (UnauthorizedAccessException ex)
        {
            var errorResponse = new Dictionary<string, string> { { "Message", ex.Message } };
            return Results.Json(errorResponse, statusCode: StatusCodes.Status403Forbidden);
        }
    }
}