using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Bus.Abstractions;
using Platform.Bus.Publisher;
using Platform.Contract.Profiles;
using Platform.Cryptography;
using Platform.Geolocation.IpResolver;

namespace Platform.Services.Background.BackgroundServices
{
    public class CollectorSubscriptionService : BackgroundService
    {
        private readonly IBusSubscriber _busSubscriber;
        private readonly IBusPublisher _busPublisher;
        private readonly IPublicIpResolver _hostLocation;
        private readonly DiffieHellmanKeyMaker _keyGenerator;
        private readonly CollectorInfo _collectorInfo;
        private readonly ILogger _logger;

        // heartbeat interval should be shorter than a coordinator cache ttl
        private readonly TimeSpan _heartbeatInterval = TimeSpan.FromMinutes(1);

        public CollectorSubscriptionService(
            IBusSubscriber busSubscriber,
            IPublicIpResolver hostLocation,
            DiffieHellmanKeyMaker keyGenerator,
            IBusPublisher busPublisher,
            CollectorInfo collectorInfo,
            ILogger<CollectorSubscriptionService> logger)
        {
            _busSubscriber = busSubscriber;
            _busPublisher = busPublisher;
            _hostLocation = hostLocation;
            _keyGenerator = keyGenerator;
            _collectorInfo = collectorInfo;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var ipAddressBytes = (await _hostLocation.FindPublicIpAddress()).GetAddressBytes();
            var publicKeyBytes = _keyGenerator.PublicKey;

            await _busSubscriber.SubscribeByHostIdentifier(_collectorInfo.Identifier, cancellationToken);

            while (true)
            {
                if (cancellationToken.IsCancellationRequested) break;

                await _busPublisher.PublishToSyncExchange(_collectorInfo, ipAddressBytes, publicKeyBytes);
                await Task.Delay(_heartbeatInterval, cancellationToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _keyGenerator.Dispose();
            _busSubscriber.Unsubscribe(cancellationToken);
            return Task.CompletedTask;
        }
    }
}