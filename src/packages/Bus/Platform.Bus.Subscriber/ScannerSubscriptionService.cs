using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Platform.Bus.Publisher;
using Platform.Tool.GeoService.Abstractions;

namespace Platform.Bus.Subscriber
{
    public class ScannerSubscriptionService : IHostedService
    {
        private readonly IBusSubscriber _busSubscriber;
        private readonly IBusPublisher _busPublisher;
        private readonly IGeoService _geoService;

        public ScannerSubscriptionService(IBusSubscriber busSubscriber, IGeoService geoService, IBusPublisher busPublisher)
        {
            _busSubscriber = busSubscriber;
            _busPublisher = busPublisher;
            _geoService = geoService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // todo: re-ran every 1h

            var geoMarker = await _geoService.FindLocalGeoMarker();
            await _busPublisher.PublishToGeoSynchronizationExchange(geoMarker);
            await _busSubscriber.SubscribeByGeoMarker(geoMarker, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _busSubscriber.Unsubscribe(cancellationToken);
            return Task.CompletedTask;
        }
    }
}