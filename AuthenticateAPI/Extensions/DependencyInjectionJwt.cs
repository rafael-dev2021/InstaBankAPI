using System.Text;
using AuthenticateAPI.Security;
using AuthenticateAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticateAPI.Extensions;

public static class DependencyInjectionJwt
{
    public static void AddDependencyInjectionJwt(this IServiceCollection service,
        IConfiguration configuration)
    {
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:SecretKey"] ?? string.Empty);

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
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        service.AddTransient<ITokenService,TokenService>();
        service.AddTransient<IAuthenticateService,AuthenticateService>();

        var jwtSettings = new JwtSettings();
        configuration.Bind("Jwt", jwtSettings);
        service.AddSingleton(jwtSettings);
        
        service.AddAuthorization();
    }
}