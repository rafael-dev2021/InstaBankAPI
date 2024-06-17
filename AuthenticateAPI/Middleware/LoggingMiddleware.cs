using System.Diagnostics;
using System.Text;

namespace AuthenticateAPI.Middleware;

public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            await LogAuditEvent(context.Request, context.Response, logger);
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation("Request Duration: {Duration}ms", stopwatch.ElapsedMilliseconds);
        }
    }

    public static async Task LogAuditEvent(HttpRequest request, HttpResponse response, ILogger logger)
    {
        var requestBody = await GetRequestBodyAsync(request);

        var requestInfo = new
        {
            Timestamp = DateTime.UtcNow,
            RequestMethod = request.Method,
            RequestPath = request.Path,
            QueryString = request.QueryString.ToString(),
            UserAgent = request.Headers.UserAgent.ToString(),
            RequestBody = requestBody,
        };

        var responseInfo = new
        {
            response.StatusCode,
        };

        logger.LogInformation(
            "Audit Event: {RequestMethod} {RequestPath}{QueryString} - UserAgent: {UserAgent} - Status {StatusCode}",
            requestInfo.RequestMethod, requestInfo.RequestPath, requestInfo.QueryString, requestInfo.UserAgent, responseInfo.StatusCode);
        logger.LogInformation("Request Body: {RequestBody}", requestInfo.RequestBody);
    }

    public static async Task<string> GetRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        _ = await request.Body.ReadAsync(buffer);
        var requestBody = Encoding.UTF8.GetString(buffer);
        request.Body.Seek(0, SeekOrigin.Begin);
        return requestBody;
    }
}