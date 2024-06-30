namespace AuthenticateAPI.Endpoints.Strategies;

public static class AuthenticationRules
{
    public static IResult? CheckAdminRole(HttpContext context)
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