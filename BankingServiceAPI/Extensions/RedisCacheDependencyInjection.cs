namespace BankingServiceAPI.Extensions;

public static class RedisCacheDependencyInjection
{
    public static void AddRedisCacheDependencyInjection(this IServiceCollection service)
    {
        var redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION");

        service.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.InstanceName = "SampleInstance";
        });
    }
}