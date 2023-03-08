using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Platform.Bus.Publisher;
using Platform.Tools.HostGeolocator;


namespace Platform.Bus.Subscriber
{
    public class ScannerSubscriptionService : IHostedService
    {
        private readonly IBusSubscriber _busSubscriber;
        private readonly IBusPublisher _busPublisher;
        private readonly IHostGeolocator _hostGeolocator;

        public ScannerSubscriptionService(IBusSubscriber busSubscriber, IHostGeolocator hostGeolocator, IBusPublisher busPublisher)
        {
            _busSubscriber = busSubscriber;
            _busPublisher = busPublisher;
            _hostGeolocator = hostGeolocator;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // todo: re-ran every 1h
            // todo: move ScannerSubscriptionService to another service

            var geoMarker = await _hostGeolocator.FindGeoMarkers();
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