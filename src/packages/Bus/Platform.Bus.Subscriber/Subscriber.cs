using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Force.Crc32;
using MemoryPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Extensions;
using Platform.Cryptography;
using Platform.Logging.Extensions;
using Platform.Primitives;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Platform.Bus.Subscriber;

public class Subscriber : IBusSubscriber
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly ExchangeCollection _exchangeCollection;
    private readonly ICryptographicService _cryptographicService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    private readonly string _queueName;

    public Subscriber(
        IConnection connection,
        IModel channel,
        ExchangeCollection exchangeCollection,
        ICryptographicService cryptographicService,
        IServiceProvider serviceProvider,
        ILogger<Subscriber> logger)
    {
        _connection = connection;
        _channel = channel;
        _exchangeCollection = exchangeCollection;
        _cryptographicService = cryptographicService;
        _serviceProvider = serviceProvider;
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
        consumer.Received += ConsumeEventAsync;

        _exchangeCollection.Exchanges.ForEach(value =>
        {
            var exchangeName = value.ExchangeTypes.ToLower();

            _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);
            _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: value.RoutingKey);

            _logger.Info($"Subscribed to exchange '{exchangeName}' with routing key '{value.RoutingKey}'");
        });

        _channel.BasicConsume(queueName, false, consumer);
    }

    public void Unsubscribe(CancellationToken cancellationToken)
    {
        _channel.Close();
        _connection.Close();
    }

    private async Task ConsumeEventAsync(object sender, BasicDeliverEventArgs arguments)
    {
        if (arguments.TryGetHeader<byte[]>(HeaderConstants.Session, out var sessionBytes))
        {
            var payload = arguments.Body.ToArray();

            if (arguments.TryGetHeader<byte[]>(HeaderConstants.Iv, out var iv)
                && arguments.TryGetHeader<byte[]>(HeaderConstants.Key, out var publicKeyAlice))
            {
                payload = await _cryptographicService.Decrypt(payload, publicKeyAlice, iv);
            }

            if (Crc32CAlgorithm.IsValidWithCrcAtEnd(payload))
            {
                try
                {
                    await Process(sessionBytes, payload.TrimEndBytes(4)); // trim from the end of 4 bytes crc32 value
                }
                catch (Exception e)
                {
                    _logger.Error($"A request processing error has occurred, '{arguments.RoutingKey}'", e);
                }

                _channel.BasicAck(arguments.DeliveryTag, false);

                return;
            }
        }

        // todo: do we need to re-process it? I guess not now (need notify to admin bot channel)

        _logger.Error($"Something went wrong, the '{HeaderConstants.Session}' headers corrupted or CRC not valid '{arguments.RoutingKey}'.");
        _channel.BasicAck(arguments.DeliveryTag, false);
    }

    /// <summary>
    /// Process request on internal service scope
    /// </summary>
    /// <param name="sessionBytes"></param>
    /// <param name="payloadBytes"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task Process(byte[] sessionBytes, byte[] payloadBytes)
    {
        using var scope = _serviceProvider.CreateScope();
        scope.ServiceProvider.GetRequiredService<SessionContext>().AddContext(sessionBytes);

        var profile = MemoryPackSerializer.Deserialize<IProfile>(payloadBytes)
                      ?? throw new InvalidOperationException("A deserialization error has occurred, profile can't be null.");

        var consumerInstance = scope.ServiceProvider.GetRequiredService(typeof(IConsumeAsync<>).MakeGenericType(profile.GetType()));
        var methodInfo = consumerInstance.GetType().GetMethod(nameof(IConsumeAsync<IProfile>.ConsumeAsync));

        await (ValueTask)methodInfo.Invoke(consumerInstance, BindingFlags.Public, null, new[] { profile }, CultureInfo.InvariantCulture);
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