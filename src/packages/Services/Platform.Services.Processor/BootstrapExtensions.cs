using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Subscriber;

namespace Platform.Services.Processor;

public static class BootstrapExtensions
{
    public static IServiceCollection AddProcessorSubscriber(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddBusConfiguration(configuration)
            .AddHostedService<ProcessorSubscriptionService>()
            .AddScoped<IBusSubscriber, Subscriber>()
            .AddBus();
}