using DotNetEnv;
using Microsoft.OpenApi.Models;

namespace BankingServiceAPI.Extensions;

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
                    builder.WithOrigins("https://localhost:7132", "http://localhost:5279")
                        .WithMethods("GET", "POST", "PUT", "DELETE")
                        .WithHeaders("Authorization", "Content-Type");
                });
        });

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "BankingServiceAPI",
                Description = "An ASP.NET Core Web API for managing BankingServiceAPI",
                TermsOfService = new Uri(openApiUrl!),
                Contact = new OpenApiContact
                {
                    Name = "Rafael Dev",
                    Email = "Rafael98kk@gmail.com",
                    Url = new Uri(openApiUrl!)
                }
            });
            
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            };
            
            c.AddSecurityDefinition(bearer, securityScheme);

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
                policy.RequireClaim("scope", "InstaBankAPI");
            });
    }
}