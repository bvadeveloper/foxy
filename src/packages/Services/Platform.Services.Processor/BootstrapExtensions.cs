using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Caching.Redis;

namespace Platform.Services.Processor;

public static class BootstrapExtensions
{
    public static IServiceCollection AddSubscriptions(this IServiceCollection services, IConfiguration configuration, params ExchangeTypes[] exchangeTypes) =>
        services
            .AddRedis(configuration)
            .AddPublisher(configuration)
            .AddHostedService<ProcessorSubscriptionService>()
            .AddScoped<IBusSubscriber, Subscriber>()
            .AddExchanges(exchangeTypes);
}