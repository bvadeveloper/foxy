using Microsoft.Extensions.Hosting;
using Platform.Bus;

namespace Platform.Services
{
    public class ProcessorSubscriptionHostedService : IHostedService
    {
        private readonly IBusSubscriber _busSubscriber;

        public ProcessorSubscriptionHostedService(IBusSubscriber busSubscriber) => _busSubscriber = busSubscriber;

        public async Task StartAsync(CancellationToken cancellationToken) => await _busSubscriber.Subscribe(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _busSubscriber.Unsubscribe(cancellationToken);
            return Task.CompletedTask;
        }
    }
}