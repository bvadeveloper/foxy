using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Logging.Extensions;
using RabbitMQ.Client;

namespace Platform.Bus.Publisher
{
    public class Publisher : IPublisher
    {
        private readonly IModel _channel;
        private readonly ILogger _logger;

        public Publisher(IModel channel, ILogger<Publisher> logger)
        {
            _channel = channel;
            _logger = logger;
        }

        public async Task Publish(byte[] body, Exchange exchange)
        {
            try
            {
                var exchangeName = exchange.ExchangeTypes.ToString();

                _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);
                _channel.BasicPublish(exchange: exchangeName,
                    routingKey: exchange.RoutingKey,
                    basicProperties: null,
                    body: body);

                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                _logger.Error($"An error was thrown while sending a request '{e.Message}'", e, ("session", "message"));
            }
        }
    }
}