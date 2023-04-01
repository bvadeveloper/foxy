using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Subscriber;
using Platform.Bus.Subscriber.EventProcessors;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Enums;

namespace Platform.Services.Collector;

public static class BootstrapExtensions
{
    public static IServiceCollection AddSubscription(this IServiceCollection services, IConfiguration configuration, ExchangeTypes exchangeType) =>
        services
            .AddBusConfiguration(configuration)
            .AddHostedService<CollectorSubscriptionService>()
            .AddScoped<IBusSubscriber, Subscriber>()
            .AddScoped<IEventProcessor, DecryptEventProcessor>()
            .AddExchangesAndRoute(exchangeType)
            .AddBus();

    public static IServiceCollection AddCollectorInfo(this IServiceCollection services, ProcessingTypes processingTypes) =>
        services.AddSingleton<CollectorInfo>(provider =>
        {
            var attribute = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var version = attribute?.InformationalVersion ?? "undefined";
            var exchangeRoute = provider.GetRequiredService<ExchangeRoute>();

            return new CollectorInfo(exchangeRoute.Value, version, processingTypes);
        });
}