using AuthenticateAPI.Context;
using Microsoft.EntityFrameworkCore;

namespace AuthenticateAPI.Extensions;

public static class DatabaseDependencyInjection
{
    public static IServiceCollection AddDatabaseDependencyInjection(this IServiceCollection service,
        IConfiguration configuration)
    {
        service.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        return service;
    }
}