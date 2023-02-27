using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Logging.Extensions;
using Platform.Serializer;
using RabbitMQ.Client;

namespace Platform.Bus.Publisher
{
    public class Publisher : IPublisher
    {
        private readonly IModel _channel;
        private readonly ISerializer _serializer;
        private readonly ILogger _logger;

        public Publisher(IModel channel, ISerializer serializer, ILogger<Publisher> logger)
        {
            _channel = channel;
            _serializer = serializer;
            _logger = logger;
        }

        public async Task Publish(object payload)
        {
            try
            {
                var ddd = _serializer.Serialize(payload);
                
                var (exchangeName, routingKey) = ReadAttribute(payload.GetType());

                _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

                var message1 = "Hello World!";

                var body = Encoding.UTF8.GetBytes(message1);

                _channel.BasicPublish(exchange: exchangeName,
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);

                Console.WriteLine($" [x] Sent '{routingKey}':'{message1}'");
            }
            catch (Exception e)
            {
                _logger.Error($"An error was thrown while sending a request '{e.Message}'", e, ("session", "message"));
            }
        }

        private static (string, string) ReadAttribute(Type type) =>
            type.GetInterfaces()
                .Where(t => Attribute.IsDefined(t, typeof(RouteAttribute)))
                .Select(t =>
                {
                    var attr = t.GetCustomAttribute<RouteAttribute>();
                    return (attr.Exchange.ToString().ToLower(), attr.Route);
                })
                .First();
    }
}