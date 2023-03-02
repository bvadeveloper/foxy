using Platform.Tools.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Platform.Tool.GeoIp;

namespace Platform.Scanners.Domain
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) =>
            services
                .AddTools(Configuration)
                .AddExchangeListeners(ExchangeTypes.Domain)
                .AddScoped<IConsumeAsync<Profile>, DomainScanner>()
                .AddScoped<IGeoIpService, GeoIpService>();
    }
}