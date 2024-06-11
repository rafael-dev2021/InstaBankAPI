using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthenticateAPI.Extensions;

public class SecurityHeadersAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.HttpContext.Response.Headers.ContainsKey("X-Content-Type-Options") == false)
        {
            context.HttpContext.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        }

        if (context.HttpContext.Response.Headers.ContainsKey("X-Frame-Options") == false)
        {
            context.HttpContext.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
        }

        if (context.HttpContext.Response.Headers.ContainsKey("X-XSS-Protection") == false)
        {
            context.HttpContext.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        }

        if (context.HttpContext.Response.Headers.ContainsKey("Referrer-Policy") == false)
        {
            context.HttpContext.Response.Headers.Append("Referrer-Policy", "no-referrer");
        }

        base.OnResultExecuting(context);
    }
}