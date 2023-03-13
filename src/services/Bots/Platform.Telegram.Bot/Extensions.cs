using System.Collections.Generic;
using System.Collections.Immutable;
using System.CommandLine;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Platform.Contract.Profiles;
using Platform.Contract.Telegram;
using Platform.Primitives;
using Platform.Telegram.Bot.Configuration;
using Platform.Telegram.Bot.Parser;
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

    internal static async Task<MessageOutput> ParseMessage(this string input)
    {
        var profiles = new List<CoordinatorProfile>();

        var rootCommand = MessageParser.MakeCommand((hosts, uris, emails, facebook, options) =>
        {
            profiles.AddRange(hosts.Select(value => CoordinatorProfile.Make(value.ToString(), ProcessingTypes.Host, options)));
            profiles.AddRange(uris.Select(value => CoordinatorProfile.Make(value.ToString(), ProcessingTypes.Domain, options)));
            profiles.AddRange(emails.Select(value => CoordinatorProfile.Make(value, ProcessingTypes.Email, options)));
            profiles.AddRange(facebook.Select(value => CoordinatorProfile.Make(value, ProcessingTypes.Facebook, options)));
        });

        var logger = new ParserLogger();
        var exitCode = await rootCommand.InvokeAsync(input, logger);
        var logOutput = $"{logger.Out}{logger.Error}";

        return new MessageOutput(exitCode == 0 && profiles.Any(), profiles.ToImmutableList(), logOutput);
    }
}