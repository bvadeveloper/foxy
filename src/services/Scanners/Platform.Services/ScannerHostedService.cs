using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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
        private ImmutableList<string> _routingKeys;
        private readonly string _hostIdentifier;

        // heartbeat interval should be shorter than a coordinator cache ttl
        private readonly TimeSpan _heartbeatInterval;

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
            _hostIdentifier = Guid.NewGuid().ToString("N");
            _routingKeys = ImmutableList<string>.Empty;
            _heartbeatInterval = TimeSpan.FromMinutes(1);
        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var location = await _hostGeolocator.FindCountryCode();

            while (true)
            {
                if (cancellationToken.IsCancellationRequested) break;

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
                    await _busPublisher.PublishToSynchronizationExchange(routingKey, _hostIdentifier);
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