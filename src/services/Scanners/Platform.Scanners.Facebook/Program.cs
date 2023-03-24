using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Platform.Geolocation.HostLocation;
using Platform.Host;
using Platform.Services.Hosts;
using Platform.Tools.Extensions;

namespace Platform.Scanners.Facebook;

internal static class Program
{
    public static async Task Main(string[] args) =>
        await Application.RunAsync(args, (services, configuration) =>
        {
            services
                .AddPublisher(configuration)
                .AddCollectorSubscription(configuration, ExchangeTypes.FacebookExchange)
                .AddHostLocationServices()
                .AddTools(configuration)
                .AddScoped<IConsumeAsync<FacebookProfile>, FacebookScanner>();
        });
}