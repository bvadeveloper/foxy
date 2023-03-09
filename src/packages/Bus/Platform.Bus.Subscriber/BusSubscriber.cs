using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
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

        private readonly string _subscriberName;

        private const string NoLocation = "*";
        private const string DefaultRoute = "default";

        public BusSubscriber(
            IConnection connection,
            IModel channel,
            ExchangeCollection exchangeCollection,
            IServiceProvider serviceProvider,
            ILogger<BusSubscriber> logger)
        {
            _connection = connection;
            _channel = channel;
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


        public ImmutableList<Exchange> ExchangeBindings { get; set; }

        public async Task Subscribe(CancellationToken cancellationToken)
        {
            var queueName = _channel.QueueDeclare(_subscriberName);
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += ConsumerOnReceived;

            _exchangeCollection.Exchanges.ForEach(value =>
            {
                var exchangeName = value.ExchangeTypes.ToLower();

                _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);
                value.RoutingKeys.ForEach(routingKey => _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey));

                _logger.Info($"Subscribed to exchange '{exchangeName}' with routing key '{value.RoutingKeys}'");
            });

            _channel.BasicConsume(queueName, false, consumer);

            await Task.Yield();
        }

        /// <summary>
        /// https://www.rabbitmq.com/tutorials/tutorial-five-dotnet.html
        /// </summary>
        /// <param name="location"></param>
        /// <param name="cancellationToken"></param>
        public async Task SubscribeByGeoLocation(string location, CancellationToken cancellationToken)
        {
            var queueName = _channel.QueueDeclare(_subscriberName);
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += ConsumerOnReceived;

            ExchangeBindings = BuildLocationRoutingKeys(_exchangeCollection.Exchanges, location);
            ExchangeBindings.ForEach(exchange =>
            {
                var exchangeName = exchange.ExchangeTypes.ToLower();

                _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);
                exchange.RoutingKeys.ForEach(routingKey => _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey));

                _logger.Info($"Subscribed to exchange '{exchangeName}' with routing key '{exchange.RoutingKeys}'");
            });

            _channel.BasicConsume(queueName, false, consumer);

            await Task.Yield();
        }

        /// <summary>
        /// Bindings should look like 'default.scannerExchange.countryCode'
        /// Notice. If no scanners in a particular location we can use ones from other location
        /// </summary>
        /// <param name="exchanges"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private static ImmutableList<Exchange> BuildLocationRoutingKeys(ImmutableList<Exchange> exchanges, string location) =>
            exchanges.Select(exchange => string.IsNullOrEmpty(location)
                ? exchange with
                {
                    // if we can't determine a location we must process any requests
                    RoutingKeys = ImmutableList.Create<string>($"{DefaultRoute}.{exchange.ExchangeTypes.ToLower()}.{NoLocation}")
                }
                : exchange with
                {
                    // by default, we must process any requests by target location and any other cross requests
                    RoutingKeys = ImmutableList.Create<string>()
                        .Add($"{DefaultRoute}.{exchange.ExchangeTypes.ToLower()}.{location}")
                        .Add($"{DefaultRoute}.{exchange.ExchangeTypes.ToLower()}.{NoLocation}")
                }).ToImmutableList();

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
                    _logger.Error($"A processing error has occurred, '{eventArgs.RoutingKey}'", e);
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