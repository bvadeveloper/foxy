using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Bus.Subscriber;

public static class ServiceProviderExtensions
{
    public static IServiceCollection AddSubscriber(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddBusConfiguration(configuration)
            .AddHostedService<SubscriptionService>()
            .AddScoped<IBusSubscriber, BusSubscriber>()
            .AddBus();
}