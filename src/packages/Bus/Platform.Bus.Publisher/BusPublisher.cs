using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Contract.Profiles.Extensions;
using Platform.Cryptography;
using Platform.Logging.Extensions;
using Platform.Primitives;
using RabbitMQ.Client;

namespace Platform.Bus.Publisher
{
    public class BusPublisher : IBusPublisher
    {
        private readonly IModel _channel;
        private readonly ICryptographicService _cryptographicService;
        private readonly ILogger _logger;

        private readonly Dictionary<string, object> _defaultHeaders;

        public BusPublisher(IModel channel, SessionContext sessionContext, ICryptographicService cryptographicService, ILogger<BusPublisher> logger)
        {
            _channel = channel;
            _cryptographicService = cryptographicService;
            _logger = logger;

            _defaultHeaders = MakeDefaultHeaders(sessionContext);
        }


        public ValueTask Publish(byte[] payload, Exchange exchange)
        {
            Publish(payload, exchange, _defaultHeaders);
            return ValueTask.CompletedTask;
        }

        public async ValueTask Publish(byte[] payload, Exchange exchange, byte[] publicKeyBob)
        {
            var publicKeyAlice = _cryptographicService.GetPublicKey();
            var (encryptedData, iv) = await _cryptographicService.Encrypt(payload, publicKeyBob);

            _defaultHeaders.Add(HeaderNames.Iv, iv);
            _defaultHeaders.Add(HeaderNames.Key, publicKeyAlice);

            Publish(encryptedData, exchange, _defaultHeaders);
        }

        private void Publish(byte[] payload, Exchange exchange, IDictionary<string, object> headers)
        {
            try
            {
                var exchangeName = exchange.ExchangeName.ToLower();

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
        }

        private static Dictionary<string, object> MakeDefaultHeaders(SessionContext sessionContext) =>
            new() { { HeaderNames.Session, Encoding.UTF8.GetBytes(sessionContext.ToString()) } };
    }
}