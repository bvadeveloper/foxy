using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Subscriber;
using Platform.Bus.Subscriber.EventProcessors;

namespace Platform.Services.Processor;

public static class BootstrapExtensions
{
    public static IServiceCollection AddProcessorSubscription(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddBusConfiguration(configuration)
            .AddHostedService<ProcessorSubscriptionService>()
            .AddScoped<IBusSubscriber, Subscriber>()
            .AddScoped<IEventProcessor, EventProcessor>()
            .AddBus();
    
    public static IServiceCollection AddReporterSubscription(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddBusConfiguration(configuration)
            .AddHostedService<ProcessorSubscriptionService>()
            .AddScoped<IBusSubscriber, Subscriber>()
            .AddScoped<IEventProcessor, DecryptEventProcessor>()
            .AddBus();
}