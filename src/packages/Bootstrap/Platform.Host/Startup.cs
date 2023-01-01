using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Host
{
    /// <summary>
    /// Registration of health check and service modules
    /// </summary>
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) => services.AddHealthChecks();

        public void Configure(IApplicationBuilder app) => app.UseHealthChecks("/status");
    }
}