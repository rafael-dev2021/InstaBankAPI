using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace AuthenticateAPI.Middleware;
 
public class LogoutHandlerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        if (context.Request.Path.Equals("/v1/auth/logout", StringComparison.OrdinalIgnoreCase))
        {
            var tokenRepository = serviceProvider.GetRequiredService<ITokenRepository>();
            var signInManager = serviceProvider.GetRequiredService<SignInManager<User>>();

            await LogoutAsync(context, tokenRepository, signInManager);
        }
        else
        {
            await next(context);
        }
    }

    private static async Task LogoutAsync(HttpContext context, ITokenRepository tokenRepository, SignInManager<User> signInManager)
    {
        var token = RecoverTokenFromRequest(context.Request);

        if (!string.IsNullOrEmpty(token))
        {
            var storedToken = await tokenRepository.FindByTokenValue(token);

            if (storedToken != null)
            {
                InvalidateToken(storedToken);
                await tokenRepository.SaveAsync();
                await signInManager.SignOutAsync();
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync("Logout successful");
                Log.Information("[LOGOUT_SUCCESS] User logged out successfully. Token invalidated: [{Token}]", token);
            }
            else
            {
                Log.Error("[TOKEN_EXPIRED] Token not found during logout: [{Token}]", token);
                await SendUnauthorizedResponse(context.Response);
            }
        }
        else
        {
            await SendUnauthorizedResponse(context.Response);
        }
    }

    private static string? RecoverTokenFromRequest(HttpRequest request)
    {
        if (request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var token = authHeader.ToString().Replace("Bearer ", string.Empty);
            Log.Debug("[RECOVERED] Token recovered from Authorization header: [{Token}]", token);
            return token;
        }

        Log.Debug("[NO_AUTH_HEADER] No Authorization header found in the request.");
        return null;
    }

    private static void InvalidateToken(Token token)
    {
        token.SetTokenRevoked(true);
        token.SetTokenExpired(true);
    }

    private static async Task SendUnauthorizedResponse(HttpResponse response)
    {
        response.StatusCode = StatusCodes.Status401Unauthorized;
        await response.WriteAsync("Token is expired");
    }
}