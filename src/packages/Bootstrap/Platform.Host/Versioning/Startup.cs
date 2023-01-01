using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Host.Versioning
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) => services.AddTransient<VersioningMiddleware>();

        public void Configure(IApplicationBuilder app) => app.UseMiddleware<VersioningMiddleware>();
    }
}
