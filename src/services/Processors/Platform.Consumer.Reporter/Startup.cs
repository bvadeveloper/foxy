using Platform.Consumer.Reporter.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;

namespace Platform.Consumer.Reporter
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) =>
            services
                .AddExchangeListeners(ExchangeTypes.Report)
                .AddScoped<IReportService, CustomerReportService>()
                .AddScoped<IConsumeAsync<Profile>, ReportConsumer>();
    }
}