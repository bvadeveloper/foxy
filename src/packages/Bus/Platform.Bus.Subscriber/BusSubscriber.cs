using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MemoryPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Platform.Contract.Profiles;
using Platform.Logging.Extensions;
using Platform.Primitives;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Platform.Bus.Subscriber
{
    public class BusSubscriber : IBusSubscriber
    {
        private readonly IConnection _connection;
        private readonly IModel _model;
        private readonly ILogger _logger;
        private readonly ExchangeCollection _exchangeCollection;
        private readonly IServiceProvider _serviceProvider;

        private readonly string _subscriberName;

        public BusSubscriber(
            IConnection connection,
            IModel model,
            ExchangeCollection exchangeCollection,
            IServiceProvider serviceProvider,
            ILogger<BusSubscriber> logger)
        {
            _connection = connection;
            _model = model;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _exchangeCollection = exchangeCollection;

            _subscriberName = MakeSubscriberName();
        }

        private static string MakeSubscriberName()
        {
            var span = AppDomain.CurrentDomain.FriendlyName.AsSpan();
            return span[(span.LastIndexOf('.') + 1)..].ToString();
        }

        /// <summary>
        /// Subscribe to consumers from assembly
        /// </summary>
        /// <param name="cancellationToken"></param>
        public async Task Subscribe(CancellationToken cancellationToken)
        {
            var queueName = _model.QueueDeclare(_subscriberName);
            var consumer = new AsyncEventingBasicConsumer(_model);
            consumer.Received += ConsumerOnReceived;

            _exchangeCollection.Exchanges.ForEach(value =>
            {
                var exchangeName = value.ExchangeTypes.ToString();

                _model.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);
                _model.QueueBind(queue: queueName, exchange: exchangeName, routingKey: value.RoutingKey);

                _logger.Info($"Subscribed to exchange {exchangeName} with routing key {value.RoutingKey}");
            });

            _model.BasicConsume(queueName, false, consumer);

            await Task.Yield();
        }

        public void Unsubscribe(CancellationToken cancellationToken)
        {
            _model.Close();
            _connection.Close();
        }

        private async Task ConsumerOnReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            if (eventArgs.BasicProperties.Headers.TryGetValue("fx-session", out var sessionBytes))
            {
                using var scope = _serviceProvider.CreateScope();
                scope.ServiceProvider.GetRequiredService<SessionContext>().AddContext((byte[])sessionBytes);

                try
                {
                    var payload = MemoryPackSerializer.Deserialize<IProfile>(eventArgs.Body.ToArray())
                                  ?? throw new InvalidOperationException("A deserialization error has occurred, profile can't be null.");

                    var consumerInstance = scope.ServiceProvider.GetRequiredService(typeof(IConsumeAsync<>).MakeGenericType(payload.GetType()));
                    var methodInfo = consumerInstance.GetType().GetMethod(nameof(IConsumeAsync<IProfile>.ConsumeAsync));

                    await (ValueTask)methodInfo.Invoke(consumerInstance, BindingFlags.Public, null, new[] { payload }, CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                    _logger.Error($"A processing error has occurred, '{eventArgs.RoutingKey}'", e);
                }

                _model.BasicAck(eventArgs.DeliveryTag, false);
                await Task.Yield();
            }
            else
            {
                _logger.Error($"Something went wrong, the 'fx-session' headers corrupted, '{eventArgs.RoutingKey}'");
                _model.BasicAck(eventArgs.DeliveryTag, false);
                // todo: do we need to re-process it, I guess not (need to notify about it to admin channel)
            }
        }
    }
}