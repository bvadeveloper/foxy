using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Bus.Subscriber.EventProcessors;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Collectors;
using Platform.Contract.Profiles.Enums;
using Platform.Cryptography;
using Platform.Geolocation.IpResolver;
using Platform.Primitives;
using Platform.Tools.Extensions;

namespace Platform.Services.Collector;

public static class BootstrapExtensions
{
    /// <summary>
    /// Subscribe collectors to bus
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="processingTypes"></param>
    /// <param name="exchangeType"></param>
    /// <returns></returns>
    public static IServiceCollection AddSubscriptions(this IServiceCollection services, IConfiguration configuration, ProcessingTypes processingTypes,
        string exchangeType) =>
        services

            // services
            .AddTools(configuration)
            .AddPublicIpResolver()

            // crypto
            .AddAesCryptographicServices()
            .AddScoped<PublicKeyHolder>()
            .AddCollectorInfo(processingTypes)

            // bus
            .AddPublisher(configuration)
            .AddScoped<IProcessorClient, ProcessorClient>()
            .AddHostedService<CollectorSubscriptionService>()
            .AddScoped<IBusSubscriber, BusSubscriber>()
            .AddScoped<IEventProcessor, EventDecryptProcessor>()
            .AddExchanges(new[] { exchangeType }, Ulid.NewUlid().ToString() );

    private static IServiceCollection AddCollectorInfo(this IServiceCollection services, ProcessingTypes processingTypes) =>
        services.AddSingleton<CollectorInfo>(provider =>
        {
            var attribute = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var version = attribute?.InformationalVersion ?? "undefined";
            var identifier = provider.GetRequiredService<ExchangeCollection>().Exchanges.First().RoutingKey;

            return new CollectorInfo(identifier, version, processingTypes);
        });
}