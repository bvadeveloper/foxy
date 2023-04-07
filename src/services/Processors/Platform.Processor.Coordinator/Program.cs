using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Subscriber;
using Platform.Bus.Subscriber.EventProcessors;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Processors;
using Platform.Cryptography;
using Platform.Geolocation.HostGeolocation;
using Platform.Host;
using Platform.Processor.Coordinator.Processors;
using Platform.Processor.Coordinator.Repositories;
using Platform.Processor.Coordinator.Strategies;
using Platform.Services.Processor;

namespace Platform.Processor.Coordinator;

internal static class Program
{
    public static async Task Main(string[] args) =>
        await Application.RunAsync(args, (services, configuration) =>
        {
            services

                // bus
                .AddSubscriptions(configuration, ExchangeNames.Coordinator, ExchangeNames.Sync)
                .AddScoped<IEventProcessor, EventProcessor>()
                
                // services
                .AddHostGeolocation()

                // crypto
                .AddAesCryptographicServices()
                .AddHostedService<CryptographicKeySynchronizationService>()

                // processors
                .AddScoped<IConsumeAsync<CoordinatorProfile>, CoordinatorProcessor>()
                .AddScoped<IConsumeAsync<SynchronizationProfile>, SynchronizationProcessor>()
                .AddScoped<ICollectorInfoRepository, CollectorInfoRepository>()

                // strategies
                .AddScoped<IProcessingStrategy, DomainProcessingStrategy>()
                .AddScoped<IProcessingStrategy, HostProcessingStrategy>()
                .AddScoped<IProcessingStrategy, EmailProcessingStrategy>()
                .AddScoped<IProcessingStrategy, FacebookProcessingStrategy>()
                .AddScoped<IStrategyFactory, StrategyFactory>();
        });
}