using System.Net.Http.Headers;

namespace BankingServiceAPI.Middleware;

public class TokenValidationMiddleware(RequestDelegate next, HttpClient httpClient)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var token))
        {
            token = token.ToString().Replace("Bearer ", string.Empty);
            var isValid = await IsTokenValidAsync(token!);

            if (!isValid)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token is expired or revoked");
                return;
            }
        }

        await next(context);
    }

    private async Task<bool> IsTokenValidAsync(string token)
    {
        const string url = "https://localhost:7074/v1/auth/revoke-token";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
}