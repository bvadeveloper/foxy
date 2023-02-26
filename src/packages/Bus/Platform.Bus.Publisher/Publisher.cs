using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Contract;
using Platform.Contract.Abstractions;
using Platform.Contract.Models;
using Platform.Logging.Extensions;
using Platform.Primitive;
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

        public async Task<Result<string>> Publish<T>(Message<T> message) where T : ITarget
        {
            try
            {
                var (exchangeName, routingKey) = ReadAttribute((IExchange)message.Payload);

                _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

                var message1 = "Hello World!";

                var body = Encoding.UTF8.GetBytes(message1);

                _channel.BasicPublish(exchange: exchangeName,
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);

                Console.WriteLine($" [x] Sent '{routingKey}':'{message1}'");

                return await Task.FromResult(new Result<string>().UseResult($"{message.Payload.Name} - processing").Ok());
            }
            catch (Exception e)
            {
                _logger.Error($"An error was thrown while sending a request '{e.Message}'", e, ("session", message));
                return new Result<string>().UseResult($"{message.Payload.Name} - can't process, something went wrong").Fail();
            }
        }

        private static (string, string) ReadAttribute<T>(T value) where T : IExchange
        {
            var exchangeType = typeof(IExchange);
            var derivedType = value.GetType().GetInterfaces().First(i => exchangeType != i && i.IsAssignableTo(exchangeType));
            var attribute = Attribute.GetCustomAttribute(derivedType, typeof(ExchangeAttribute), true) as ExchangeAttribute;

            return (attribute.Exchange.ToString().ToLower(), attribute.Route);
        }
    }
}