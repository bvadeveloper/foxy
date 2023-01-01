using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Platform.Logging.Extensions;

namespace Platform.Bus.Subscriber
{
    public class MessageDispatcher : IAutoSubscriberMessageDispatcher
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger _logger;

        public MessageDispatcher(IServiceProvider provider, ILogger<MessageDispatcher> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public void Dispatch<TMessage, TConsumer>(TMessage message, CancellationToken cancellationToken = new())
            where TMessage : class
            where TConsumer : class, IConsume<TMessage>
        {
            using var scope = _provider.CreateScope();
            try
            {
                _logger.Trace($"Request for '{typeof(TConsumer).Name}' with type '{typeof(TMessage)}', {message}");
                var consumer = scope.ServiceProvider.GetRequiredService<TConsumer>();
                consumer.Consume(message, cancellationToken);
            }
            catch (InvalidOperationException oex)
            {
                _logger.Error($"Can't resolve consumer '{typeof(TConsumer).Name}' for type '{typeof(TMessage)}' {message}", oex);
            }
            catch (Exception ex)
            {
                _logger.Error("Processing failed", ex);
            }
        }

        public async Task DispatchAsync<TMessage, TConsumer>(TMessage message,
            CancellationToken cancellationToken = new())
            where TMessage : class
            where TConsumer : class, IConsumeAsync<TMessage>
        {
            using var scope = _provider.CreateScope();
            try
            {
                _logger.Trace($"Request for '{typeof(TConsumer).Name}' with type '{typeof(TMessage)}' {message}");
                var consumer = scope.ServiceProvider.GetRequiredService<TConsumer>();
                await consumer.ConsumeAsync(message, cancellationToken);
            }
            catch (InvalidOperationException oex)
            {
                _logger.Error($"Can't resolve consumer '{typeof(TConsumer).Name}' for type '{typeof(TMessage)}'", oex);
            }
            catch (Exception e)
            {
                _logger.Error($"Processing failed, {e.Message}", e);
            }
        }
    }
}