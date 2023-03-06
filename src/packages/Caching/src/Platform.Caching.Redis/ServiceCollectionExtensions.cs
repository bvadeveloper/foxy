using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Caching.Abstractions;
using Platform.Caching.Redis.Internal;

namespace Platform.Caching.Redis;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration) =>
        services.AddSingleton<IRedisConnection>(provider =>
            {
                var connectionString = configuration.GetSection("RedisConnectionString").Value;
                return new RedisConnection(connectionString);
            })
            .AddSingleton(provider => new Lazy<IRedisConnection>(provider.GetRequiredService<IRedisConnection>))
            .AddScoped<ICacheDataService, RedisCacheDataService>();
}