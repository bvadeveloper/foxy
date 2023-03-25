using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Enums;
using Platform.Geolocation.IpResolver;
using Platform.Host;
using Platform.Services.Background;
using Platform.Tools.Extensions;

namespace Platform.Collector.Domain;

internal static class Program
{
    public static async Task Main(string[] args) =>
        await Application.RunAsync(args, (services, configuration) =>
        {
            services
                .AddPublicIpResolver()
                .AddPublisher(configuration)
                .AddCollectorSubscriber(configuration, ExchangeTypes.DomainExchange)
                .AddTools(configuration)
                .AddCollectorInfo(ProcessingTypes.Domain)
                .AddScoped<IConsumeAsync<DomainProfile>, DomainScanner>();
        });
}