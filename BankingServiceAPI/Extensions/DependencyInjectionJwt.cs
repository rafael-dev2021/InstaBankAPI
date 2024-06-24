using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace BankingServiceAPI.Extensions;

public static class DependencyInjectionJwt
{
    public static void AddDependencyInjectionJwt(this IServiceCollection service)
    {
        Env.Load();

        var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");
        var issuer = Environment.GetEnvironmentVariable("ISSUER");
        var audience = Environment.GetEnvironmentVariable("AUDIENCE");

        var key = Encoding.ASCII.GetBytes(secretKey!);

        service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
    }
}