using Microsoft.Extensions.Hosting;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Tools.HostGeolocator;

namespace Platform.Services
{
    public class ScannerSubscriptionHostedService : IHostedService
    {
        private readonly IBusSubscriber _busSubscriber;
        private readonly IBusPublisher _busPublisher;
        private readonly IHostGeolocator _hostGeolocator;

        public ScannerSubscriptionHostedService(IBusSubscriber busSubscriber, IHostGeolocator hostGeolocator, IBusPublisher busPublisher)
        {
            _busSubscriber = busSubscriber;
            _busPublisher = busPublisher;
            _hostGeolocator = hostGeolocator;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // todo: re-ran every 1h
            // todo: move ScannerSubscriptionService to another service

            var markers = await _hostGeolocator.FindGeoMarkers();
            await _busPublisher.PublishToGeoSynchronizationExchange(markers);
            await _busSubscriber.SubscribeByGeoMarker(markers, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _busSubscriber.Unsubscribe(cancellationToken);
            return Task.CompletedTask;
        }
    }
}