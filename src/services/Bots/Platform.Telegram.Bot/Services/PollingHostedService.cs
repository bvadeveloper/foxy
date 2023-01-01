using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Platform.Bus.Publisher.Abstractions;
using Platform.Contract.Collector;
using Platform.Logging.Extensions;
using Platform.Primitive;
using Platform.Telegram.Bot.Extensions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Platform.Telegram.Bot.Services
{
    public class PollingHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly QueuedUpdateReceiver _updateReceiver;
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger _logger;

        public PollingHostedService(
            IServiceProvider serviceProvider,
            QueuedUpdateReceiver updateReceiver,
            ITelegramBotClient botClient,
            ILogger<PollingHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _updateReceiver = updateReceiver;
            _botClient = botClient;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await foreach (var update in _updateReceiver.WithCancellation(cancellationToken))
                {
                    if (update.Message is not { } message) continue;
                    try
                    {
                        // todo: workaround for resolving scoped service from singleton lifetime scope 
                        using var scope = _serviceProvider.CreateScope();

                        var messageContext = scope.ServiceProvider.GetRequiredService<TraceContext>();
                        var publishClient = scope.ServiceProvider.GetRequiredService<IPublishClient>();

                        var profiles = message.Text
                            .ToTarget()
                            .Validate()
                            .MakeProfiles<DomainCollectorProfile>(messageContext
                                .FillSession(message.Chat.Id));

                        var confirmations = await publishClient.Publish(profiles).Extract();

                        await _botClient.SendTextMessageAsync(
                            message.Chat, string.Join(Environment.NewLine, confirmations), cancellationToken: cancellationToken);
                    }
                    catch (Exception e)
                    {
                        await _botClient.SendTextMessageAsync(
                            message.Chat,
                            $"sorry, input not recognized, {e.Message.ToLower()}", cancellationToken: cancellationToken);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error($"An error was thrown while updating the message receiver. '{e.Message}'", e);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Trace($"Messenger host '{nameof(PollingHostedService)}' stopped");
            return Task.CompletedTask;
        }
    }
}