using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Platform.Primitives;
using Platform.Telegram.Bot.Configuration;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Platform.Telegram.Bot;

public static class Extensions
{
    internal static IServiceCollection AddTelegramBot(this IServiceCollection services,
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

    internal static SessionContext AddChatId(this SessionContext context, long id)
    {
        context.ChatId = id.ToString();
        return context;
    }

    internal static string MakeUserKey(User? user) => $"{user.FirstName}:{user.Id}";

    internal static async Task Say(this ITelegramBotClient botClient, Chat chat, string message,
        CancellationToken token)
    {
        await botClient.SendChatActionAsync(chat, ChatAction.Typing, cancellationToken: token);
        await botClient.SendTextMessageAsync(chat, message, cancellationToken: token);
    }

    internal static bool IsAny(this string[]? values) =>
        values switch
        {
            null => false,
            _ => values.Length switch
            {
                0 => false,
                _ => true
            }
        };
}