using System;
using System.Collections.Generic;
using System.Linq;
using Platform.Validation.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Platform.Contract;
using Platform.Contract.Abstractions;
using Platform.Primitive;
using Platform.Telegram.Bot.Configuration;
using Platform.Telegram.Bot.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

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
                .AddHostedService<PollingHostedService>();

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

        internal static TraceContext FillSession(this TraceContext context, long id)
        {
            context.ChatId = id;

            return context;
        }

        internal static TargetModel ToTarget(this string message) =>
            new()
            {
                Targets = message
                    .Split(Environment.NewLine)
                    .AsParallel()
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Select(t =>
                        t.Trim()
                            .TrimEnd('/')
                            .Replace("http://", "")
                            .Replace("https://", ""))
                    .ToArray()
            };

        internal static TargetModel Validate(this TargetModel model)
        {
            var validator = new TargetModelValidator();
            var result = validator.Validate(model);
            return result.IsValid
                ? model
                : throw new ArgumentException(
                    "validation not passed, please use only valid domain names or IPv4 addresses");
        }

        internal static string MakeInput(User? user) => $"{user.FirstName}:{user.Id}";
        
        internal static IEnumerable<TProfile> MakeProfiles<TProfile>(this TargetModel targetModel, TraceContext sessionContext)
            where TProfile : ITargetProfile, IToolProfile, new() =>
            targetModel.Targets.Select(target => new TProfile { Target = target, TraceContext = sessionContext, Tools = Array.Empty<string>() });
    }
}