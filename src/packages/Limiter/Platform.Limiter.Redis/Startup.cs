using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Limiter.Redis.Abstractions;
using Platform.Limiter.Redis.Models;

namespace Platform.Limiter.Redis
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) =>
            services
                .Configure<List<LimiterModel>>(options => Configuration.GetSection("Limiter").Bind(options))
                .AddSingleton<IRequestLimiter, RequestLimiter>();
    }
}