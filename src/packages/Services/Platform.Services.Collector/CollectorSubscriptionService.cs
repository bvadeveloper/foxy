using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Contract.Profiles;
using Platform.Cryptography;
using Platform.Geolocation.IpResolver;

namespace Platform.Services.Collector;

public class CollectorSubscriptionService : BackgroundService
{
    private readonly IBusSubscriber _busSubscriber;
    private readonly IBusPublisher _busPublisher;
    private readonly IPublicIpResolver _hostLocation;
    private readonly ICryptographicService _cryptographicService;
    private readonly CollectorInfo _collectorInfo;
    private readonly ILogger _logger;

    // heartbeat interval should be shorter than a coordinator cache ttl
    private readonly TimeSpan _heartbeatInterval = TimeSpan.FromMinutes(1);

    public CollectorSubscriptionService(
        IBusSubscriber busSubscriber,
        IPublicIpResolver hostLocation,
        ICryptographicService cryptographicService,
        IBusPublisher busPublisher,
        CollectorInfo collectorInfo,
        ILogger<CollectorSubscriptionService> logger)
    {
        _busSubscriber = busSubscriber;
        _busPublisher = busPublisher;
        _hostLocation = hostLocation;
        _cryptographicService = cryptographicService;
        _collectorInfo = collectorInfo;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var ipAddress = (await _hostLocation.FindPublicIpAddress()).GetAddressBytes();
        var publicKey = _cryptographicService.GetPublicKey();

        _busSubscriber.SubscribeByHostIdentifier(_collectorInfo.Identifier, cancellationToken);

        while (true)
        {
            if (cancellationToken.IsCancellationRequested) break;

            await _busPublisher.PublishToSyncExchange(_collectorInfo, ipAddress, publicKey);
            await Task.Delay(_heartbeatInterval, cancellationToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _cryptographicService.Dispose();
        _busSubscriber.Unsubscribe(cancellationToken);
        return Task.CompletedTask;
    }
}