﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus.Abstractions;
using Platform.Cryptography;
using Platform.Logging.Extensions;
using Platform.Primitives;
using RabbitMQ.Client;

namespace Platform.Bus.Publisher
{
    public class Publisher : IBusPublisher
    {
        private readonly IModel _channel;
        private readonly ILogger _logger;
        private readonly ICryptographicService _cryptographicService;
        private readonly SessionContext _sessionContext;

        public Publisher(IModel channel, SessionContext sessionContext, ICryptographicService cryptographicService, ILogger<Publisher> logger)
        {
            _channel = channel;
            _sessionContext = sessionContext;
            _cryptographicService = cryptographicService;
            _logger = logger;
        }

        public ValueTask Publish(byte[] payload, Exchange exchange)
        {
            try
            {
                var sessionBytes = Encoding.UTF8.GetBytes(_sessionContext.ToString());
                var exchangeName = exchange.ExchangeTypes.ToString().ToLower();

                var props = _channel.CreateBasicProperties();
                props.DeliveryMode = 1;
                props.Headers = new Dictionary<string, object>();
                props.Headers.Add("fx-session", sessionBytes);

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

        public ValueTask Publish(byte[] payload, Exchange exchange, byte[] publicKey)
        {
            var encryptedPayload = _cryptographicService.Encrypt(payload, publicKey, out byte[] iv);
            return Publish(payload, exchange);
        }
    }
}