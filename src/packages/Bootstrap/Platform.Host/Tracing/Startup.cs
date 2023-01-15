using System.Runtime.CompilerServices;
using Platform.Primitive;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Host.Tracing
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped(_ => new StrongBox<SessionContext?>(SessionContext.Init()));
            services.AddScoped<SessionContext>(provider =>
            {
                var strongBox = provider.GetService<StrongBox<SessionContext?>>();
                return strongBox?.Value ?? SessionContext.Init();
            });
        }
    }
}