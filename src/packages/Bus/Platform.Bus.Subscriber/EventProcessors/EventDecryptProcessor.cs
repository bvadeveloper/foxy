using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Force.Crc32;
using MemoryPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Platform.Bus.Constants;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Extensions;
using Platform.Cryptography;
using Platform.Logging.Extensions;
using Platform.Primitives;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Platform.Bus.Subscriber.EventProcessors;

public class EventDecryptProcessor : IEventProcessor
{
    private readonly IModel _channel;
    private readonly ICryptographicService _cryptographicService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    public EventDecryptProcessor(
        IModel channel,
        IServiceProvider serviceProvider,
        ICryptographicService cryptographicService,
        ILogger<EventDecryptProcessor> logger)
    {
        _channel = channel;
        _serviceProvider = serviceProvider;
        _cryptographicService = cryptographicService;
        _logger = logger;
    }

    public async Task Process(object sender, BasicDeliverEventArgs arguments)
    {
        if (arguments.TryGetHeader<byte[]>(HeaderConstants.Session, out var sessionBytes)
            && arguments.TryGetHeader<byte[]>(HeaderConstants.Iv, out var iv) 
            && arguments.TryGetHeader<byte[]>(HeaderConstants.Key, out var publicKeyAlice))
        {
            var payload = await _cryptographicService.Decrypt(arguments.Body.ToArray(), publicKeyAlice, iv);

            if (Crc32CAlgorithm.IsValidWithCrcAtEnd(payload))
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    scope.ServiceProvider.GetRequiredService<SessionContext>().AddContext(sessionBytes);
                    scope.ServiceProvider.GetRequiredService<PublicKeyHolder>().AddPublicKey(publicKeyAlice);

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

        _logger.Error($"Something went wrong, the '{HeaderConstants.Session}' headers corrupted or CRC not valid '{arguments.RoutingKey}'.");
        _channel.BasicAck(arguments.DeliveryTag, false);
    }
}