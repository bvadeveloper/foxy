using Platform.Consumer.Reporter.Abstractions;
using Platform.Consumer.Reporter.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Consumer.Reporter
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) =>
            services
                .AddScoped<ReportConsumer>()
                .AddScoped<IReportService, CustomerReportService>();
    }
}