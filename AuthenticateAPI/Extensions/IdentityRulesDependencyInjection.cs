using AuthenticateAPI.Context;
using AuthenticateAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthenticateAPI.Extensions;

public static class IdentityRulesDependencyInjection
{
    public static void AddIdentityRulesDependencyInjection(this IServiceCollection service)
    {
        service.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        service.AddDistributedMemoryCache();
        service.AddSession();
        service.AddMemoryCache();

        service.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        service.Configure<IdentityOptions>(options =>
        {
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(15);
            options.Lockout.MaxFailedAccessAttempts = 3;
            options.Lockout.AllowedForNewUsers = true;
        });

        service.Configure<PasswordOptions>(options =>
        {
            options.RequireDigit = true;
            options.RequireLowercase = true;
            options.RequireUppercase = true;
            options.RequireNonAlphanumeric = true;
            options.RequiredLength = 8;
            options.RequiredUniqueChars = 6;
        });

        service.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => { policy.RequireRole("Admin"); });
        });

        service.AddHttpsRedirection(options => { options.HttpsPort = null; });

        service.AddHttpContextAccessor();
        service.AddMvc(options => { options.Filters.Add(new SecurityHeadersAttribute()); });

        service.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromMinutes(15);
        });

        service.AddAntiforgery(options => { options.HeaderName = "X-CSRF-TOKEN"; });

        service.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.IsEssential = true;

            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        });
    }
}