using Microsoft.Extensions.Hosting;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Geolocation.HostGeolocation;

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
            // todo: add healthcheck

            var countryCode = await _hostGeolocator.FindCountryCode();

            // examples: default.domain.usa or default.facebook.gb
            await _busSubscriber.SubscribeByGeoLocation(countryCode, cancellationToken);
            var bindings = _busSubscriber.ExchangeBindings;
            await _busPublisher.PublishToGeoSynchronizationExchange(countryCode);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _busSubscriber.Unsubscribe(cancellationToken);
            return Task.CompletedTask;
        }
    }
}