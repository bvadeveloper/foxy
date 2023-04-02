using System;
using System.Threading;
using System.Threading.Tasks;
using BotMessageParser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Limiter.Redis.Abstractions;
using Platform.Logging.Extensions;
using Platform.Primitives;
using Platform.Telegram.Bot.Extensions;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Platform.Telegram.Bot;

public class PollingMessageService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly QueuedUpdateReceiver _updateReceiver;
    private readonly ITelegramBotClient _botClient;
    private readonly IRequestLimiter _requestLimiter;
    private readonly IMessageParser _messageParser;
    private readonly ILogger _logger;

    public PollingMessageService(
        IServiceProvider serviceProvider,
        QueuedUpdateReceiver updateReceiver,
        ITelegramBotClient botClient,
        IRequestLimiter requestLimiter,
        IMessageParser messageParser,
        ILogger<PollingMessageService> logger)
    {
        _serviceProvider = serviceProvider;
        _updateReceiver = updateReceiver;
        _botClient = botClient;
        _requestLimiter = requestLimiter;
        _messageParser = messageParser;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await foreach (var update in _updateReceiver.WithCancellation(cancellationToken))
        {
            if (update.Message is not { } message || update.Message.Text is null) continue;

            if (message.From is { IsBot: true })
            {
                await _botClient.Say(message.Chat, "messages from the bots are not currently supported", cancellationToken);
                continue;
            }

            try
            {
                _logger.Trace($"Raw text '{message.Text}'");
                
                var parseResult = await _messageParser.Parse(message.Text!);
                if (parseResult.IsValid)
                {
                    using var scope = _serviceProvider.CreateScope();
                    scope.ServiceProvider.GetRequiredService<SessionContext>().AddChatId(message.Chat.Id);
                    var publisher = scope.ServiceProvider.GetRequiredService<IBusPublisher>();
                    
                    foreach (var profile in parseResult.Profiles)
                    {
                        if (await _requestLimiter.Acquire(message.From.MakeUserKey()))
                        {
                            await _botClient.Say(message.Chat, "Request limit reached, please try again in a couple of minutes", cancellationToken);
                            break;
                        }

                        await publisher.PublishToCoordinatorExchange(profile);
                        await _botClient.Say(message.Chat, $"{profile.TargetNames} - wait for a while foxy sniffing out this target", cancellationToken);
                    }
                }
                else
                {
                    await _botClient.Say(message.Chat, parseResult.ValidationInfo, cancellationToken);
                }
            }
            catch (Exception e)
            {
                _logger.Error($"An error was thrown while message processing. '{e.Message}'", e);
            }
        }
    }
}