using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MemoryPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Platform.Bus.Abstractions;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Extensions;
using Platform.Cryptography;
using Platform.Logging.Extensions;
using Platform.Primitives;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using static Force.Crc32.Crc32CAlgorithm;

namespace Platform.Bus.Subscriber
{
    public class BusSubscriber : IBusSubscriber
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger _logger;
        private readonly ExchangeCollection _exchangeCollection;
        private readonly IServiceProvider _serviceProvider;
        private readonly ICryptographicService _cryptographicService;
        private readonly string _subscriberName;

        public BusSubscriber(
            IConnection connection,
            IModel channel,
            ExchangeCollection exchangeCollection,
            IServiceProvider serviceProvider,
            ICryptographicService cryptographicService,
            ILogger<BusSubscriber> logger)
        {
            _connection = connection;
            _channel = channel;
            _logger = logger;
            _cryptographicService = cryptographicService;
            _serviceProvider = serviceProvider;
            _exchangeCollection = exchangeCollection;
            _subscriberName = MakeSubscriberName();
        }

        private static string MakeSubscriberName()
        {
            var span = AppDomain.CurrentDomain.FriendlyName.AsSpan();
            var slice = span[(span.LastIndexOf('.') + 1)..];
            var spanSliced = new Span<char>(new char[slice.Length]);
            slice.ToLowerInvariant(spanSliced);
            
            return spanSliced.ToString();
        }

        public async Task Subscribe(CancellationToken cancellationToken)
        {
            var queueName = _channel.QueueDeclare(_subscriberName);
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += ConsumerOnReceived;

            _exchangeCollection.Exchanges.ForEach(value =>
            {
                var exchangeName = value.ExchangeTypes.ToLower();

                _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);
                _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: value.RoutingKey);

                _logger.Info($"Subscribed to exchange '{exchangeName}' with routing key '{value.RoutingKey}'");
            });

            _channel.BasicConsume(queueName, false, consumer);

            await Task.Yield();
        }

        /// <summary>
        /// https://www.rabbitmq.com/tutorials/tutorial-five-dotnet.html
        /// </summary>
        /// <param name="routingKey"></param>
        /// <param name="cancellationToken"></param>
        public async Task SubscribeByHostIdentifier(string routingKey, CancellationToken cancellationToken)
        {
            var queueName = _channel.QueueDeclare(_subscriberName);
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += ConsumerOnReceived;

            _exchangeCollection.Exchanges.ForEach(exchange =>
            {
                var exchangeName = exchange.ExchangeTypes.ToLower();

                _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);
                _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);

                _logger.Info($"Subscribed to exchange '{exchangeName}' with routing key '{exchange.RoutingKey}'");
            });

            _channel.BasicConsume(queueName, false, consumer);

            await Task.Yield();
        }

        public void Unsubscribe(CancellationToken cancellationToken)
        {
            _channel.Close();
            _connection.Close();
        }

        private async Task ConsumerOnReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            var payload = eventArgs.Body.ToArray();

            if (eventArgs.BasicProperties.Headers.TryGetValue("fx-session", out var sessionBytes)
                && IsValidWithCrcAtEnd(payload))
            {
                using var scope = _serviceProvider.CreateScope();
                scope.ServiceProvider.GetRequiredService<SessionContext>().AddContext((byte[])sessionBytes);

                try
                {
                    var profile = MemoryPackSerializer.Deserialize<IProfile>(payload.AsSpan()[..(payload.Length - 4)])
                                  ?? throw new InvalidOperationException("A deserialization error has occurred, profile can't be null.");

                    var consumerInstance = scope.ServiceProvider.GetRequiredService(typeof(IConsumeAsync<>).MakeGenericType(profile.GetType()));
                    var methodInfo = consumerInstance.GetType().GetMethod(nameof(IConsumeAsync<IProfile>.ConsumeAsync));

                    await (ValueTask)methodInfo.Invoke(consumerInstance, BindingFlags.Public, null, new[] { profile }, CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                    _logger.Error($"A request processing error has occurred, '{eventArgs.RoutingKey}'", e);
                }

                _channel.BasicAck(eventArgs.DeliveryTag, false);
                await Task.Yield();
            }
            else
            {
                _logger.Error($"Something went wrong, the 'fx-session' headers corrupted or CRC not valid '{eventArgs.RoutingKey}'.");
                _channel.BasicAck(eventArgs.DeliveryTag, false);
                // todo: do we need to re-process it, I guess not (need to notify about it to admin channel)
            }
        }
    }
}