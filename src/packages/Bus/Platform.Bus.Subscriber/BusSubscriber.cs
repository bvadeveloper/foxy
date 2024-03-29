using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Platform.Bus.Subscriber.EventProcessors;
using Platform.Logging.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Platform.Bus.Subscriber;

public class BusSubscriber : IBusSubscriber
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly ExchangeCollection _exchangeCollection;
    private readonly IEventProcessor _eventProcessor;
    private readonly ILogger _logger;
    
    private readonly string _queueName;

    public BusSubscriber(
        IConnection connection,
        IModel channel,
        ExchangeCollection exchangeCollection,
        IEventProcessor eventProcessor,
        ILogger<BusSubscriber> logger)
    {
        _connection = connection;
        _channel = channel;
        _exchangeCollection = exchangeCollection;
        _eventProcessor = eventProcessor;
        _logger = logger;

        _queueName = MakeQueueName();
    }


    /// <summary>
    /// https://www.rabbitmq.com/tutorials/tutorial-five-dotnet.html
    /// </summary>
    /// <param name="cancellationToken"></param>
    public void Subscribe(CancellationToken cancellationToken)
    {
        var queueName = _channel.QueueDeclare(_queueName);
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += _eventProcessor.Process;

        _exchangeCollection.Exchanges.ForEach(value =>
        {
            _channel.ExchangeDeclare(exchange: value.ExchangeName, type: ExchangeType.Topic);
            _channel.QueueBind(queue: queueName, exchange: value.ExchangeName, routingKey: value.RoutingKey);

            _logger.Info($"Subscribed to exchange '{value.ExchangeName}' with routing key '{value.RoutingKey}'");
        });

        _channel.BasicConsume(queueName, false, consumer);
    }

    public void Unsubscribe(CancellationToken cancellationToken)
    {
        _channel.Close();
        _connection.Close();
    }

    private static string MakeQueueName()
    {
        var assemblyNameSpan = AppDomain.CurrentDomain.FriendlyName.AsSpan();
        var croppedName = assemblyNameSpan[(assemblyNameSpan.LastIndexOf('.') + 1)..];
        var buffer = new Span<char>(new char[croppedName.Length]);
        croppedName.ToLowerInvariant(buffer);

        return buffer.ToString();
    }
}