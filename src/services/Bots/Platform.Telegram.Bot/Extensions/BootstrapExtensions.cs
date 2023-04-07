using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Caching.Redis;
using Platform.Telegram.Bot.Clients;
using Platform.Telegram.Bot.Configuration;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Platform.Telegram.Bot.Extensions;

internal static class BootstrapExtensions
{
    internal static IServiceCollection AddSubscriptions(this IServiceCollection services, IConfiguration configuration, params string[] exchangeNames) =>
        services
            .AddRedis(configuration)
            .AddPublisher(configuration)
            .AddHostedService<SubscriptionService>()
            .AddScoped<IBusSubscriber, BusSubscriber>()
            .AddExchanges(exchangeNames)

            .AddScoped<ICoordinatorClient, CoordinatorClient>();
            

    internal static IServiceCollection AddTelegram(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<BotConfiguration>(options =>
            configuration.GetSection("Telegram").Bind(options));

        services
            .AddSingleton<ITelegramBotClient>(provider =>
            {
                var messengerConfiguration = provider.GetRequiredService<IOptions<BotConfiguration>>().Value;
                return new TelegramBotClient(messengerConfiguration.ApiKey);
            })
            .AddSingleton(provider =>
            {
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = { } // receive all update types
                };
                var botClient = provider.GetRequiredService<ITelegramBotClient>();
                return new QueuedUpdateReceiver(botClient, receiverOptions);
            })
            .AddHostedService<PollingMessageService>();

        return services;
    }
}