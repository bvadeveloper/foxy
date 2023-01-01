using System.Runtime.CompilerServices;
using Platform.Primitive;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Host.Tracing
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped(_ => new StrongBox<TraceContext?>(TraceContext.Empty()));
            services.AddScoped<TraceContext>(sp =>
            {
                var box = sp.GetService<StrongBox<TraceContext?>>();
                return box?.Value ?? TraceContext.Init();
            });
        }
    }
}
