using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Platform.Telegram.Bot.Configuration;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Platform.Telegram.Bot.Extensions;

internal static class BootstrapExtensions
{
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