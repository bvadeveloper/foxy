using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Logging.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Platform.Bus.Subscriber
{
    public class Subscriber : ISubscriber
    {
        private readonly IConnection _connection;
        private readonly IModel _model;
        private readonly ILogger _logger;
        private readonly RoutesHolder _routesHolder;

        private readonly string _subscriberName;

        public Subscriber(IConnection connection, IModel model, ILogger<Subscriber> logger, RoutesHolder routesHolder)
        {
            _connection = connection;
            _model = model;
            _logger = logger;
            _routesHolder = routesHolder;

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

        /// <summary>
        /// Subscribe to consumers from assembly
        /// </summary>
        /// <param name="cancellationToken"></param>
        public async Task Subscribe(CancellationToken cancellationToken)
        {
            var queueName = _model.QueueDeclare(_subscriberName);

            var consumer = new AsyncEventingBasicConsumer(_model);
            consumer.Received += async (ch, ea) =>
            {
                // decode 
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                // process message
                _logger.Info($" [x] Received '{routingKey}':'{message}'");

                _model.BasicAck(ea.DeliveryTag, false);
                await Task.Yield();
            };
            await Task.Yield();

            _routesHolder.Values.ForEach(value =>
            {
                var exchangeName = value.exchangeTypes.ToString().ToLower();

                _model.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);
                _model.QueueBind(queue: queueName, exchange: exchangeName, routingKey: value.route);

                _logger.Info($"Subscribed to exchange {exchangeName} with routing key {value.route}");
            });

            _model.BasicConsume(queueName, false, consumer);
        }

        public void Unsubscribe(CancellationToken cancellationToken)
        {
            _model.Close();
            _connection.Close();
        }
    }
}