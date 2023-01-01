using Platform.Consumer.Scanner.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Tools.Extensions;

namespace Platform.Consumer.Scanner
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) =>
            services
                .AddScoped<ScanConsumer>()
                .AddTools(Configuration)
                .AddHealthChecks();
    }
}