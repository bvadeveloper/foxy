using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Platform.Contract;
using Platform.Contract.Abstractions;
using Platform.Contract.Models;
using Platform.Contract.Models.Bot;
using Platform.Primitive;
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

        /// <summary>
        /// Split string by chunk length
        /// </summary>
        internal static IEnumerable<string> SplitBy(this string message, int chunkLength)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException($"'{nameof(message)}' can't be null or empty");
            }

            if (chunkLength < 1)
            {
                throw new ArgumentException($"'{nameof(chunkLength)}' can't be less than 1");
            }

            for (var i = 0; i < message.Length; i += chunkLength)
            {
                if (chunkLength + i > message.Length)
                {
                    chunkLength = message.Length - i;
                }

                yield return message.Substring(i, chunkLength);
            }
        }

        internal static SessionContext FillSession(this SessionContext context, long id)
        {
            context.ChatId = id;
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

        internal static ITarget ToTarget(this string message) =>
            IPAddress.TryParse(message, out var ipAddress)
                ? new IpTarget(Name: ipAddress.ToString())
                : new DomainTarget(Name: message);

        internal static string MakeUserKey(User? user) => $"{user.FirstName}:{user.Id}";

        internal static async Task Say(this ITelegramBotClient botClient, Chat chat, string message,
            CancellationToken token)
        {
            await botClient.SendChatActionAsync(chat, ChatAction.Typing, cancellationToken: token);
            await botClient.SendTextMessageAsync(chat, message, cancellationToken: token);
        }
    }
}