using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Caching.Redis;
using Platform.Contract.Profiles;
using Platform.Cryptography;
using Platform.Geolocation.HostGeolocation;
using Platform.Host;
using Platform.Processor.Coordinator.Processors;
using Platform.Processor.Coordinator.Strategies;
using Platform.Services.Background;

namespace Platform.Processor.Coordinator;

internal static class Program
{
    public static async Task Main(string[] args) =>
        await Application.RunAsync(args, (services, configuration) =>
        {
            services
                
                .AddPublisher(configuration)
                .AddProcessorSubscriber(configuration)
                .AddExchangeListeners(ExchangeTypes.CoordinatorExchange, ExchangeTypes.SynchronizationExchange)
                
                .AddRedis(configuration)
                .AddHostGeolocation()
                .AddCryptographicServices()
                
                .AddScoped<IConsumeAsync<CoordinatorProfile>, CoordinatorProcessor>()
                .AddScoped<IConsumeAsync<SynchronizationProfile>, SynchronizationProcessor>()

                // strategies
                .AddScoped<IProcessingStrategy, DomainProcessingStrategy>()
                .AddScoped<IProcessingStrategy, HostProcessingStrategy>()
                .AddScoped<IProcessingStrategy, EmailProcessingStrategy>()
                .AddScoped<IProcessingStrategy, FacebookProcessingStrategy>()
                .AddScoped<IStrategyFactory, StrategyFactory>();
        });
}