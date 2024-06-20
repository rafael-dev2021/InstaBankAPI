using System.Security.Claims;
using AuthenticateAPI.Models;
using AuthenticateAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticateAPI.Security;

public class SecurityFilter(RequestDelegate next, ILogger<SecurityFilter> logger)
{
    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        try
        {
            var tokenService = serviceProvider.GetRequiredService<ITokenService>();
            var tokenRepository = serviceProvider.GetRequiredService<ITokenRepository>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            var token = RecoverTokenFromRequest(context.Request);
            if (token != null)
            {
                await HandleAuthentication(context, token, tokenService, tokenRepository, userManager);
            }

            await next(context);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ERROR_FILTER] Error processing security filter: {Message}", e.Message);
            throw;
        }
    }

    private string? RecoverTokenFromRequest(HttpRequest request)
    {
        if (request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var token = authHeader.ToString().Replace("Bearer ", string.Empty);
            logger.LogDebug("[RECOVERED] Token recovered from Authorization header: {Token}", token);
            return token;
        }

        logger.LogDebug("[NO_AUTH_HEADER] No Authorization header found in the request.");
        return null;
    }

    private async Task HandleAuthentication(HttpContext context, string token, ITokenService tokenService, ITokenRepository tokenRepository, UserManager<User> userManager)
    {
        try
        {
            var email = tokenService.ValidateToken(token)?.Identity?.Name;
            if (email != null)
            {
                var user = await userManager.FindByEmailAsync(email);
                var isTokenValid = await IsTokenValid(tokenRepository, token);

                if (user != null && isTokenValid)
                {
                    SetAuthenticationInSecurityContext(context, user);
                }
                else
                {
                    LogError(context, token);
                }
            }
        }
        catch (SecurityTokenException e)
        {
            await HandleInvalidToken(context.Response, e);
            LogError(context, token);
        }
    }

    private void LogError(HttpContext context, string token)
    {
        logger.LogError(
            "[TOKEN_FAILED] User-Agent: {UserAgent}. IP Address: {IpAddress}. Validation failed for token: {Token}",
            GetUserAgent(context), GetIpAddress(context), token);
    }

    private static string? GetIpAddress(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString();
    }

    private static string GetUserAgent(HttpContext context)
    {
        return context.Request.Headers.UserAgent.ToString();
    }

    private static async Task<bool> IsTokenValid(ITokenRepository tokenRepository, string token)
    {
        var dbToken = await tokenRepository.FindByTokenValue(token);
        return dbToken is { TokenExpired: false, TokenRevoked: false };
    }

    private static void SetAuthenticationInSecurityContext(HttpContext context, User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!)
        };

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
        context.User = claimsPrincipal;
    }

    private async Task HandleInvalidToken(HttpResponse response, SecurityTokenException e)
    {
        response.StatusCode = StatusCodes.Status401Unauthorized;
        await response.WriteAsync(e.Message);
        logger.LogWarning("[TOKEN_INVALID] Invalid token detected: {Message}", e.Message);
    }
}