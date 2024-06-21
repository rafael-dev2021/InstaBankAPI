using DotNetEnv;
using Microsoft.OpenApi.Models;

namespace AuthenticateAPI.Extensions;

public static class OpenApiExtensions
{
    public static void AddOpenApiExtensions(this IServiceCollection services)
    {
        Env.Load();
        var openApiUrl = Environment.GetEnvironmentVariable("OPENAPI_URL");
        const string bearer = "Bearer";

        services.AddAuthorizationBuilder()
            .AddPolicy("Admin", policy => { policy.RequireRole("Admin"); });

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder.WithOrigins("https://localhost:7074", "http://localhost:5082")
                        .WithMethods("GET", "POST", "PUT", "DELETE")
                        .WithHeaders("Authorization", "Content-Type");
                });
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

            c.AddSecurityDefinition(bearer, new OpenApiSecurityScheme
            {
                Description = $"'{bearer}' [space] your token",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = bearer
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = bearer
                        },
                        Scheme = "oauth2",
                        Name = bearer,
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