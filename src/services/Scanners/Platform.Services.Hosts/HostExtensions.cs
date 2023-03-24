using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Platform.Cryptography;

namespace Platform.Services.Hosts;

public static class HostExtensions
{
    public static IServiceCollection AddProcessorSubscription(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddSingleton<DiffieHellmanKeyGenerator>() // move to other extension
            .AddBusConfiguration(configuration)
            .AddHostedService<ProcessorBackgroundService>() // move to other extension
            .AddScoped<IBusSubscriber, BusSubscriber>()
            .AddBus();

    public static IServiceCollection AddCollectorSubscription(this IServiceCollection services, IConfiguration configuration, ExchangeTypes exchangeType) =>
        services
            .AddSingleton<DiffieHellmanKeyGenerator>() // move to other extension
            .AddBusConfiguration(configuration)
            .AddHostedService<CollectorBackgroundService>() // move to other extension
            .AddScoped<IBusSubscriber, BusSubscriber>()
            .AddExchangeListeners(exchangeType)
            .AddBus();


    public static IServiceCollection AddCollectorInfo(this IServiceCollection services, CollectorTypes collectorTypes) =>
        services.AddSingleton<CollectorInfo>(_ =>
        {
            var assembly = Assembly.GetEntryAssembly();
            var attr = assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var version = attr?.InformationalVersion ?? "undefined";

            return new CollectorInfo(Guid.NewGuid().ToString("N"), version, collectorTypes);
        });
}