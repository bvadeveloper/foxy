using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Platform.Host;
using Platform.Tool.GeoIp;
using Platform.Tools.Extensions;

namespace Platform.Scanners.Email;

internal static class Program
{
    public static async Task Main(string[] args) =>
        await Application.RunAsync(args, (services, configuration) =>
        {
            services
                .AddPublisher(configuration)
                .AddSubscriber(configuration)
                .AddExchangeListeners(ExchangeTypes.Email)
                .AddTools(configuration)
                .AddScoped<IConsumeAsync<Profile>, EmailScanner>()
                .AddScoped<IGeoIpService, GeoIpService>();
        });
}