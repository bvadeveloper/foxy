using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Bus.Subscriber.EventProcessors;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Enums;
using Platform.Contract.Profiles.Extensions;
using Platform.Cryptography;
using Platform.Geolocation.IpResolver;
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
        ExchangeTypes exchangeType) =>
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
            .AddScoped<ICollectorPublisher, CollectorPublisher>()
            .AddHostedService<CollectorSubscriptionService>()
            .AddScoped<IBusSubscriber, BusSubscriber>()
            .AddScoped<IEventProcessor, DecryptEventProcessor>()
            .AddExchanges(new[] { exchangeType }, ProfileExtensions.MakeIdentifier());

    private static IServiceCollection AddCollectorInfo(this IServiceCollection services, ProcessingTypes processingTypes) =>
        services.AddSingleton<CollectorInfo>(provider =>
        {
            var attribute = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var version = attribute?.InformationalVersion ?? "undefined";
            var identifier = provider.GetRequiredService<ExchangeCollection>().Exchanges.First().RoutingKey;

            return new CollectorInfo(identifier, version, processingTypes);
        });
}