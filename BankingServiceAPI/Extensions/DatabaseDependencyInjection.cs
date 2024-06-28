using BankingServiceAPI.Context;
using DotNetEnv;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BankingServiceAPI.Extensions;

public static class DatabaseDependencyInjection
{
    public static void AddDatabaseDependencyInjection(this IServiceCollection services)
    {
        Env.Load();

        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new SqlConnectionStringBuilder(configurationBuilder.GetConnectionString("DefaultConnection"))
        {
            Password = Environment.GetEnvironmentVariable("DB_PASSWORD")
        };

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.ConnectionString,
                x => x.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
    }
}