using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Caching.Abstractions;
using Platform.Caching.Redis.Internal;

namespace Platform.Caching.Redis;

public static class BootstrapExtensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration) =>
        services.AddSingleton<IRedisConnection>(provider =>
            {
                var connectionString = configuration.GetSection("RedisConnectionString").Value;
                return new RedisConnection(connectionString ?? throw new InvalidOperationException("Redis connection string can't be null."));
            })
            .AddSingleton(provider => new Lazy<IRedisConnection>(provider.GetRequiredService<IRedisConnection>))
            .AddScoped<ICacheDataService, RedisCacheDataService>();
}