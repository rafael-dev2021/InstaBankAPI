using System.Net;
using System.Text.Json;
using BankingServiceAPI.Exceptions;

namespace BankingServiceAPI.Middleware;

public class ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        logger.LogDebug(exception, "An error occurred while requesting: {Message}", exception.Message);

        logger.LogError(exception, "An error occurred during the request.");

        var response = context.Response;
        response.ContentType = "application/json";

        var responseModel = new { message = "An error occurred. Please try again later." };
        var statusCode = HttpStatusCode.InternalServerError;

        switch (exception)
        {
            case BalanceInsufficientException:
                statusCode = HttpStatusCode.BadRequest;
                responseModel = new { message = exception.Message };
                break;
            case AccountNotFoundException:
            case GetIdNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                responseModel = new { message = exception.Message };
                break;
            case BankAccountDtoServiceException:
                statusCode = HttpStatusCode.BadRequest;
                responseModel = new { message = exception.Message };
                break;
        }

        response.StatusCode = (int)statusCode;
        return response.WriteAsync(JsonSerializer.Serialize(responseModel));
    }
}