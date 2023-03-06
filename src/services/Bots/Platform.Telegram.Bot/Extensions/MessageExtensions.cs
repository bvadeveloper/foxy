using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Platform.Primitives;
using Platform.Telegram.Bot.Configuration;
using Platform.Telegram.Bot.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Platform.Telegram.Bot.Extensions
{
    public static class MessageExtensions
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

        internal static IEnumerable<string> SplitMessage(this string message) =>
            message
                .Split(Environment.NewLine)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t =>
                    t.Trim()
                        .TrimEnd('/')
                        .Replace("https://", "")
                        .Replace("http://", ""))
                .ToImmutableList();
        
        internal static string MakeUserKey(User? user) => $"{user.FirstName}:{user.Id}";

        internal static async Task Say(this ITelegramBotClient botClient, Chat chat, string message,
            CancellationToken token)
        {
            await botClient.SendChatActionAsync(chat, ChatAction.Typing, cancellationToken: token);
            await botClient.SendTextMessageAsync(chat, message, cancellationToken: token);
        }
    }
}