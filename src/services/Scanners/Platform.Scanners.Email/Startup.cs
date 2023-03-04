using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Platform.Tool.GeoIp;
using Platform.Tools.Extensions;

namespace Platform.Scanners.Email
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) =>
            services
                .AddTools(Configuration)
                .AddExchangeListeners(ExchangeTypes.Email)
                .AddScoped<IConsumeAsync<Profile>, EmailScanner>()
                .AddScoped<IGeoIpService, GeoIpService>();
    }
}