using System.Collections.Immutable;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Geolocation.HostGeolocation;
using Platform.Logging.Extensions;

namespace Platform.Services
{
    public class ScannerHostedService : BackgroundService
    {
        private readonly IBusSubscriber _busSubscriber;
        private readonly IBusPublisher _busPublisher;
        private readonly IHostGeolocation _hostGeolocator;
        private readonly ILogger _logger;
        private readonly TimeSpan _heartbeatInterval;
        private ImmutableList<string> _routingKeys;
        private readonly Guid _hostId; 

        public ScannerHostedService(
            IBusSubscriber busSubscriber,
            IHostGeolocation hostGeolocator,
            IBusPublisher busPublisher,
            ILogger<ScannerHostedService> logger)
        {
            _busSubscriber = busSubscriber;
            _busPublisher = busPublisher;
            _hostGeolocator = hostGeolocator;
            _logger = logger;
            _routingKeys = ImmutableList<string>.Empty;
            _heartbeatInterval = TimeSpan.FromSeconds(10);
            _hostId = Guid.NewGuid();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var location = await _hostGeolocator.FindCountryCode();

            while (true)
            {
                if (_routingKeys.IsEmpty)
                {
                    try
                    {
                        await _busSubscriber.SubscribeByLocation(location, cancellationToken);
                        _routingKeys = _busSubscriber.ExchangeBindings.SelectMany(exchange => exchange.RoutingKeys).ToImmutableList();
                    }
                    catch (Exception e)
                    {
                        _logger.Error($"It looks like subscriptions for '{nameof(ScannerHostedService)}' failed, let's try again, '{e.Message}'", e);
                        _routingKeys = ImmutableList<string>.Empty;
                        continue;
                    }
                }

                foreach (var routingKey in _routingKeys)
                {
                    await _busPublisher.PublishToSynchronizationExchange(routingKey, _hostId);
                }

                await Task.Delay(_heartbeatInterval, cancellationToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _busSubscriber.Unsubscribe(cancellationToken);
            return Task.CompletedTask;
        }
    }
}