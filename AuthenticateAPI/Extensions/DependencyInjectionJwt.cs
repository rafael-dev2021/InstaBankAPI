using System.Text;
using AuthenticateAPI.Security;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticateAPI.Extensions;

public static class DependencyInjectionJwt
{
    public static void AddDependencyInjectionJwt(this IServiceCollection service)
    {
        Env.Load();

        var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");
        var issuer = Environment.GetEnvironmentVariable("ISSUER");
        var audience = Environment.GetEnvironmentVariable("AUDIENCE");
        var expirationTokenMinutes = Environment.GetEnvironmentVariable("EXPIRES_TOKEN");
        var expirationRefreshTokenMinutes = Environment.GetEnvironmentVariable("EXPIRES_REFRESHTOKEN");

        var key = Encoding.ASCII.GetBytes(secretKey!);

        service.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
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

        var jwtSettings = new JwtSettings
        {
            SecretKey = secretKey,
            Issuer = issuer,
            Audience = audience,
            ExpirationTokenMinutes = int.Parse(expirationTokenMinutes!),
            RefreshTokenExpirationMinutes = int.Parse(expirationRefreshTokenMinutes!)
        };
        service.AddSingleton(jwtSettings);
    }
}