using DotNetEnv;
using Microsoft.OpenApi.Models;

namespace AuthenticateAPI.Extensions;

public static class OpenApiExtensions
{
    public static void AddOpenApiExtensions(this IServiceCollection services)
    {
        Env.Load();
        var openApiUrl = Environment.GetEnvironmentVariable("OPENAPI_URL");
        
        services.AddAuthorizationBuilder()
            .AddPolicy("Admin", policy => { policy.RequireRole("Admin"); });

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "AuthenticateAPI",
                Description = "An ASP.NET Core Web API for managing AuthenticateAPI",
                TermsOfService = new Uri(openApiUrl!),
                Contact = new OpenApiContact
                {
                    Name = "Rafael Dev",
                    Email = "Rafael98kk@gmail.com",
                    Url = new Uri(openApiUrl!)
                }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"'Bearer' [space] your token",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });

        services.AddAuthorizationBuilder()
            .AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "AuthenticateAPI");
            });
    }
}