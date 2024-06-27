using System.Net.Http.Headers;

namespace BankingServiceAPI.Middleware;

public class TokenValidationMiddleware(RequestDelegate next, HttpClient httpClient)
{
    private const string BaseUrl = "https://localhost:7074";

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var token))
        {
            token = token.ToString().Replace("Bearer ", string.Empty);
            var isRevokedTokenValidAsync = await IsRevokedTokenValidAsync(token!);
            var isExpiredTokenValidAsync = await IsExpiredTokenValidAsync(token!);

            if (!isRevokedTokenValidAsync || !isExpiredTokenValidAsync)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token is expired or revoked");
                return;
            }
        }

        await next(context);
    }

    private async Task<bool> IsRevokedTokenValidAsync(string token)
    {
        const string url = $"{BaseUrl}/v1/auth/revoked-token";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    private async Task<bool> IsExpiredTokenValidAsync(string token)
    {
        const string url = $"{BaseUrl}/v1/auth/expired-token";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
}