using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Caching.Abstractions;
using Platform.Caching.Redis.Internal;

namespace Platform.Caching.Redis
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) =>
            services.AddSingleton<IRedisConnection>(provider =>
                {
                    var configuration = provider.GetService<IConfiguration>();
                    var connectionString = configuration.GetSection("RedisConnectionString").Value;
                    var redisCacheService = new RedisConnection(connectionString);

                    return redisCacheService;
                })
                .AddSingleton(provider => new Lazy<IRedisConnection>(provider.GetService<IRedisConnection>))
                .AddScoped<ICacheDataService, RedisCacheDataService>();
    }
}