using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus.Abstractions;
using Platform.Contract.Profiles.Extensions;
using Platform.Cryptography;
using Platform.Logging.Extensions;
using Platform.Primitives;
using RabbitMQ.Client;

namespace Platform.Bus.Publisher
{
    public class Publisher : IBusPublisher
    {
        private readonly IModel _channel;
        private readonly ICryptographicService _cryptographicService;
        private readonly ILogger _logger;

        private readonly Dictionary<string, object> _defaultHeaders;

        public Publisher(IModel channel, SessionContext sessionContext, ICryptographicService cryptographicService, ILogger<Publisher> logger)
        {
            _channel = channel;
            _cryptographicService = cryptographicService;
            _logger = logger;

            _defaultHeaders = MakeDefaultHeaders(sessionContext);
        }
        
        
        public ValueTask Publish(byte[] payload, Exchange exchange) => Publish(payload, exchange, _defaultHeaders);

        public async ValueTask Publish(byte[] payload, Exchange exchange, byte[] publicKeyBob)
        {
            var publicKeyAlice = _cryptographicService.GetPublicKey();
            var encryptedPayload = await _cryptographicService.Encrypt(payload, publicKeyBob, out byte[] iv);

            _defaultHeaders.Add("fx-iv", iv);
            _defaultHeaders.Add("fx-key", publicKeyAlice);

            await Publish(encryptedPayload, exchange, _defaultHeaders);
        }

        private ValueTask Publish(byte[] payload, Exchange exchange, IDictionary<string, object> headers)
        {
            try
            {
                var exchangeName = exchange.ExchangeTypes.ToLower();

                var props = _channel.CreateBasicProperties();
                props.DeliveryMode = 1;
                props.Headers = headers;

                _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);
                _channel.BasicPublish(exchange: exchangeName,
                    routingKey: exchange.RoutingKey,
                    basicProperties: props,
                    body: payload);
            }
            catch (Exception e)
            {
                _logger.Error($"An error was thrown while sending a request '{e.Message}'", e, ("exchange", exchange));
                throw;
            }

            return ValueTask.CompletedTask;
        }

        private static Dictionary<string, object> MakeDefaultHeaders(SessionContext sessionContext) =>
            new() { { "fx-session", Encoding.UTF8.GetBytes(sessionContext.ToString()) } };
    }
}