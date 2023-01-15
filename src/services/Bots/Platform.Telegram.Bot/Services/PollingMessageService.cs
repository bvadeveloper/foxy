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
using static Platform.Telegram.Bot.Extensions.MessageExtensions;

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
                        await _botClient.Say(message.Chat, "messages from the bots are not currently supported", cancellationToken);
                        continue;
                    }

                    try
                    {
                        _logger.Trace($"Input '{message.Text}'");
                        
                        using var scope = _serviceProvider.CreateScope();
                        var publisher = scope.ServiceProvider.GetService<IPublisher>();
                        var session = scope.ServiceProvider.GetService<SessionContext>().FillSession(message.Chat.Id);
                        
                        var validationResults = _validationFactory.Validate(message.Text!.ToTargets(session));

                        foreach (var (target, isValid) in validationResults)
                        {
                            if (isValid)
                            {
                                if (await _requestLimiter.Acquire(MakeInput(message.From)))
                                {
                                    await _botClient.Say(message.Chat, "request limit reached, please try again in a couple of minutes", cancellationToken);
                                    break;
                                }
                                
                                var confirmation = await publisher.Publish(target).Extract();
                                await _botClient.Say(message.Chat, confirmation, cancellationToken);
                            }
                            else
                            {
                                await _botClient.Say(message.Chat, $"{target.Value} - validation failed, please use valid domain names or IP address", cancellationToken);
                            }
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

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Trace($"Messenger host '{nameof(PollingMessageService)}' stopped");
            return Task.CompletedTask;
        }
    }
}