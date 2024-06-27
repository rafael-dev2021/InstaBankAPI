namespace BankingServiceAPI.Endpoints.Strategies;

public static class AuthenticationRules
{
    public static IResult? CheckAuthenticationAndAuthorization(HttpContext context, bool requireAuthentication)
    {
        if (!requireAuthentication) return null;

        if (context.User.Identity!.IsAuthenticated) return CheckAdminRole(context);
        var errorResponse = new Dictionary<string, string> { { "Message", "User is not authenticated." } };
        return Results.Json(errorResponse, statusCode: StatusCodes.Status401Unauthorized);
    }

    private static IResult? CheckAdminRole(HttpContext context)
    {
        if (context.User.IsInRole("Admin")) return null;

        var errorResponse = new Dictionary<string, string>
            { { "Message", "User does not have the required authorization." } };
        return Results.Json(errorResponse, statusCode: StatusCodes.Status403Forbidden);
    }

    public static IResult HandleUnauthorizedAccessException(UnauthorizedAccessException ex)
    {
        var errorResponse = new Dictionary<string, string> { { "Message", ex.Message } };
        return Results.Json(errorResponse, statusCode: StatusCodes.Status403Forbidden);
    }
}