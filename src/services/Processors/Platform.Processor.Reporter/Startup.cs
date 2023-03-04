using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;

namespace Platform.Processor.Reporter
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) =>
            services
                .AddExchangeListeners(ExchangeTypes.Report)
                .AddScoped<IReportBuilder, ReportBuilder>()
                .AddScoped<IConsumeAsync<Profile>, ReportProcessor>();
    }
}