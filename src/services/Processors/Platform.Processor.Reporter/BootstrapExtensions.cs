using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Caching.Redis;
using Platform.Processor.Reporter.Clients;
using Platform.Services.Processor;

namespace Platform.Processor.Reporter;

public static class BootstrapExtensions
{
    public static IServiceCollection AddSubscriptions(this IServiceCollection services, IConfiguration configuration, params string[] exchangeNames) =>
        services
            .AddRedis(configuration)
            .AddPublisher(configuration)
            .AddHostedService<SubscriptionService>()
            .AddScoped<IBusSubscriber, BusSubscriber>()
            .AddExchanges(exchangeNames)

            .AddScoped<ITelegramClient, TelegramClient>();
}