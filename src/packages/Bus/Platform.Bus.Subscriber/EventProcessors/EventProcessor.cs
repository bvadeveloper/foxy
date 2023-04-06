using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Force.Crc32;
using MemoryPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Extensions;
using Platform.Logging.Extensions;
using Platform.Primitives;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Platform.Bus.Subscriber.EventProcessors;

public class EventProcessor : IEventProcessor
{
    private readonly IModel _channel;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    public EventProcessor(IModel channel, IServiceProvider serviceProvider, ILogger<EventProcessor> logger)
    {
        _channel = channel;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Process(object sender, BasicDeliverEventArgs arguments)
    {
        if (arguments.TryGetHeader<byte[]>(HeaderNames.Session, out var sessionBytes))
        {
            var payload = arguments.Body.ToArray();

            if (Crc32CAlgorithm.IsValidWithCrcAtEnd(payload))
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    scope.ServiceProvider.GetRequiredService<SessionContext>().AddContext(sessionBytes);

                    var profile = MemoryPackSerializer.Deserialize<IProfile>(payload.TrimEndBytes(4))
                                  ?? throw new InvalidOperationException("A deserialization error has occurred, profile can't be null.");

                    var consumerInstance = scope.ServiceProvider.GetRequiredService(typeof(IConsumeAsync<>).MakeGenericType(profile.GetType()));
                    var methodInfo = consumerInstance.GetType().GetMethod(nameof(IConsumeAsync<IProfile>.ConsumeAsync));

                    await (Task)methodInfo.Invoke(consumerInstance, BindingFlags.Public, null, new[] { profile }, CultureInfo.InvariantCulture);

                    _channel.BasicAck(arguments.DeliveryTag, false);
                    return;
                }
                catch (Exception e)
                {
                    _logger.Error($"A request processing error has occurred, '{arguments.RoutingKey}'", e);
                }
            }
        }

        // todo: do we need to re-process it? I guess not now (need notify to admin bot channel)

        _logger.Error($"Something went wrong, the '{HeaderNames.Session}' headers corrupted or CRC not valid '{arguments.RoutingKey}'.");
        _channel.BasicAck(arguments.DeliveryTag, false);
    }
}