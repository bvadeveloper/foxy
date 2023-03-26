using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Enums;

namespace Platform.Services.Collector;

public static class BootstrapExtensions
{
    public static IServiceCollection AddCollectorSubscriber(this IServiceCollection services, IConfiguration configuration, ExchangeTypes exchangeType) =>
        services
            .AddBusConfiguration(configuration)
            .AddHostedService<CollectorSubscriptionService>()
            .AddScoped<IBusSubscriber, Subscriber>()
            .AddExchangeListeners(exchangeType)
            .AddBus();

    public static IServiceCollection AddCollectorInfo(this IServiceCollection services, ProcessingTypes processingTypes) =>
        services.AddSingleton<CollectorInfo>(_ =>
        {
            var attribute = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var version = attribute?.InformationalVersion ?? "undefined";
            var hostIdentifier = Guid.NewGuid().ToString("N");

            return new CollectorInfo(hostIdentifier, version, processingTypes);
        });
}