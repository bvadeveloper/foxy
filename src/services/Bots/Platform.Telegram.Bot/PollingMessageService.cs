using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Contract.Profiles;
using Platform.Contract.Telegram;
using Platform.Limiter.Redis.Abstractions;
using Platform.Logging.Extensions;
using Platform.Primitives;
using Telegram.Bot;
using Telegram.Bot.Polling;
using static Platform.Telegram.Bot.Extensions;

namespace Platform.Telegram.Bot;

public class PollingMessageService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly QueuedUpdateReceiver _updateReceiver;
    private readonly ITelegramBotClient _botClient;
    private readonly IRequestLimiter _requestLimiter;
    private readonly ILogger _logger;

    public PollingMessageService(
        IServiceProvider serviceProvider,
        QueuedUpdateReceiver updateReceiver,
        ITelegramBotClient botClient,
        IRequestLimiter requestLimiter,
        ILogger<PollingMessageService> logger)
    {
        _serviceProvider = serviceProvider;
        _updateReceiver = updateReceiver;
        _botClient = botClient;
        _requestLimiter = requestLimiter;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var update in _updateReceiver.WithCancellation(cancellationToken))
            {
                if (update.Message is not { } message) continue;

                if (message.From is { IsBot: true })
                {
                    await _botClient.Say(message.Chat, "messages from the bots are not currently supported", cancellationToken);
                    continue;
                }

                try
                {
                    _logger.Trace($"Raw input '{message.Text}'");

                    using var scope = _serviceProvider.CreateScope();
                    scope.ServiceProvider.GetRequiredService<SessionContext>().AddChatId(message.Chat.Id);
                    var publisher = scope.ServiceProvider.GetRequiredService<IBusPublisher>();

                    var output = await message.Text!.ParseMessage();
                    if (output.IsValid)
                    {
                        foreach (var profile in output.Profiles)
                        {
                            if (await _requestLimiter.Acquire(MakeUserKey(message.From)))
                            {
                                await _botClient.Say(message.Chat, "request limit reached, please try again in a couple of minutes", cancellationToken);
                                break;
                            }

                            await publisher.PublishToCoordinatorExchange(profile);
                            await _botClient.Say(message.Chat, $"{profile.TargetName} - wait for a while foxy sniffing out smth for you", cancellationToken);
                        }
                    }
                    else
                    {
                        await _botClient.Say(message.Chat, output.ValidationInfo, cancellationToken);
                    }
                }
                catch (Exception e)
                {
                    _logger.Error($"An error was thrown while message processing. '{e.Message}'", e);
                }
            }
        }
        catch (Exception e)
        {
            _logger.Error($"An error was thrown while updating the message receiver. '{e.Message}'", e);
        }
    }
}