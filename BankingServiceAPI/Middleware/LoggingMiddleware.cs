using System.Diagnostics;
using System.Text;
using Serilog;

namespace BankingServiceAPI.Middleware;

public class LoggingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            await LogAuditEvent(context.Request, context.Response);
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            Log.Information("Request Duration: [{Duration}]ms", stopwatch.ElapsedMilliseconds);
        }
    }

    public static async Task LogAuditEvent(HttpRequest request, HttpResponse response)
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

        Log.Information(
            "Audit Event: [{RequestMethod}] - [{RequestPath}{QueryString}] - UserAgent: [{UserAgent}] - Status [{StatusCode}]",
            requestInfo.RequestMethod, requestInfo.RequestPath, requestInfo.QueryString, requestInfo.UserAgent, responseInfo.StatusCode);
        Log.Information("Request Body: [{RequestBody}]", requestInfo.RequestBody ?? "No body");
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