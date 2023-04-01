using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Enums;
using Platform.Cryptography;
using Platform.Geolocation.IpResolver;
using Platform.Host;
using Platform.Services.Collector;
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
                .AddSubscription(configuration, ExchangeTypes.Domain)
                .AddCollectorInfo(ProcessingTypes.Domain)
                .AddTools(configuration)
                
                .AddAesCryptographicServices()
                .AddScoped<PublicKeyHolder>()
                
                .AddScoped<IConsumeAsync<DomainProfile>, DomainScanner>();
        });
}