using Platform.Consumer.Collector.Consumers;
using Platform.Tools.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Consumer.Collector
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) =>
            services
                .AddScoped<CollectorConsumer>()
                .AddTools(Configuration);
    }
}