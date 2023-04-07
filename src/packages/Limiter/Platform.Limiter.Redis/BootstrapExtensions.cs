using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Limiter.Redis.Abstractions;

namespace Platform.Limiter.Redis
{
    public static class BootstrapExtensions
    {
        public static IServiceCollection AddRequestLimiter(this IServiceCollection services, IConfiguration configuration) =>
            services
                .Configure<List<UserPermission>>(options => configuration.GetSection("Limiter").Bind(options))
                .AddSingleton<IRequestLimiter, RequestLimiter>()
                .AddSingleton<IPermissionRepository, PermissionRepository>();
    }
}