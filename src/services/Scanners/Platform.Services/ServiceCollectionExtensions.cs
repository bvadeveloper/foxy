using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Subscriber;

namespace Platform.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSubscription(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddBusConfiguration(configuration)
            .AddHostedService<ProcessorHostedService>()
            .AddScoped<IBusSubscriber, BusSubscriber>()
            .AddBus();

    public static IServiceCollection AddScannerSubscription(this IServiceCollection services, IConfiguration configuration, ExchangeTypes exchangeType) =>
        services
            .AddBusConfiguration(configuration)
            .AddHostedService<ScannerHostedService>()
            .AddScoped<IBusSubscriber, BusSubscriber>()
            .AddExchangeListeners(exchangeType)
            .AddBus();
}