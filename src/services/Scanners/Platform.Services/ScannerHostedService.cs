using Microsoft.Extensions.Hosting;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Tools.HostGeolocation;

namespace Platform.Services
{
    public class ScannerHostedService : IHostedService
    {
        private readonly IBusSubscriber _busSubscriber;
        private readonly IBusPublisher _busPublisher;
        private readonly IHostGeolocation _hostGeolocator;

        public ScannerHostedService(IBusSubscriber busSubscriber, IHostGeolocation hostGeolocator, IBusPublisher busPublisher)
        {
            _busSubscriber = busSubscriber;
            _busPublisher = busPublisher;
            _hostGeolocator = hostGeolocator;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // todo: re-ran every 1h

            var countryCode = await _hostGeolocator.FindCountryCode();
            await _busPublisher.PublishToGeoSynchronizationExchange(countryCode);
            await _busSubscriber.SubscribeByGeoMarker(countryCode, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _busSubscriber.Unsubscribe(cancellationToken);
            return Task.CompletedTask;
        }
    }
}