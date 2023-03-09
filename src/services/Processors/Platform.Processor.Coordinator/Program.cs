using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Caching.Redis;
using Platform.Contract.Profiles;
using Platform.Host;
using Platform.Services;
using Platform.Geolocation.TargetGeolocation;

namespace Platform.Processor.Coordinator;

internal static class Program
{
    public static async Task Main(string[] args) =>
        await Application.RunAsync(args, (services, configuration) =>
        {
            services
                .AddRedis(configuration)
                .AddPublisher(configuration)
                .AddSubscription(configuration)
                .AddExchangeListeners(ExchangeTypes.Coordinator, ExchangeTypes.GeoSynchronization)
                .AddTargetGeolocation()
                .AddScoped<IConsumeAsync<Profile>, CoordinatorProcessor>();
        });
}