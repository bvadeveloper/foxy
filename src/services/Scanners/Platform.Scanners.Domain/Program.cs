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

namespace Platform.Scanners.Domain;

internal static class Program
{
    public static async Task Main(string[] args) =>
        await Application.RunAsync(args, (services, configuration) =>
        {
            services
                .AddHostLocationServices()
                .AddPublisher(configuration)
                .AddCollectorSubscription(configuration, ExchangeTypes.DomainExchange)
                .AddTools(configuration)
                .AddCollectorInfo(CollectorTypes.DomainScanner)
                .AddScoped<IConsumeAsync<DomainProfile>, DomainScanner>();
        });
}