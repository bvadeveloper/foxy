using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Platform.Bus.Publisher.Abstractions;
using Platform.Limiter.Redis.Abstractions;
using Platform.Logging.Extensions;
using Platform.Primitive;
using Platform.Telegram.Bot.Extensions;
using Platform.Validation.Fluent.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace Platform.Telegram.Bot.Services
{
    public class PollingMessageService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly QueuedUpdateReceiver _updateReceiver;
        private readonly ITelegramBotClient _botClient;
        private readonly IRequestLimiter _requestLimiter;
        private readonly IValidationFactory _validationFactory;
        private readonly ILogger _logger;

        public PollingMessageService(
            IServiceProvider serviceProvider,
            QueuedUpdateReceiver updateReceiver,
            ITelegramBotClient botClient,
            IRequestLimiter requestLimiter,
            IValidationFactory validationFactory,
            ILogger<PollingMessageService> logger)
        {
            _serviceProvider = serviceProvider;
            _updateReceiver = updateReceiver;
            _botClient = botClient;
            _requestLimiter = requestLimiter;
            _validationFactory = validationFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await foreach (var update in _updateReceiver.WithCancellation(cancellationToken))
                {
                    if (update.Message is not { } message) continue;

                    if (message.From is { IsBot: true })
                    {
                        await _botClient.SendTextMessageAsync(message.Chat,
                            "Messages from the bots are not currently supported",
                            cancellationToken: cancellationToken);

                        continue;
                    }

                    try
                    {
                        // todo: workaround for resolving scoped service from singleton lifetime scope 
                        using var scope = _serviceProvider.CreateScope();

                        var context = scope.ServiceProvider.GetRequiredService<SessionContext>().FillSession(message.Chat.Id);
                        var publishClient = scope.ServiceProvider.GetRequiredService<IPublishClient>();

                        var targets = _validationFactory.Validate(message.Text.ToTargets(context));

                        foreach (var target in targets)
                        {
                            if (await _requestLimiter.Acquire(MessageExtensions.MakeInput(message.From)))
                            {
                                await _botClient.SendTextMessageAsync(
                                    message.Chat, "Sorry, request limit reached, try after a couple of minutes...",
                                    cancellationToken: cancellationToken);

                                break;
                            }

                            await _botClient.SendChatActionAsync(message.Chat, ChatAction.Typing,
                                cancellationToken: cancellationToken);

                            var confirmations = await publishClient.Publish(target).Extract();

                            await _botClient.SendTextMessageAsync(message.Chat,
                                string.Join(Environment.NewLine, confirmations),
                                cancellationToken: cancellationToken);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Error($"An error was thrown while message processing. '{e.Message}'", e);

                        await _botClient.SendTextMessageAsync(
                            message.Chat,
                            $"sorry, input not recognized, {e.Message.ToLower()}",
                            cancellationToken: cancellationToken);
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
            _logger.Trace($"Messenger host '{nameof(PollingMessageService)}' stopped");
            return Task.CompletedTask;
        }
    }
}