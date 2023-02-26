using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Platform.Bus.Subscriber
{
    public class HostedService : IHostedService
    {
        private readonly ISubscriber _busSubscriber;

        public HostedService(ISubscriber busSubscriber) => _busSubscriber = busSubscriber;

        public async Task StartAsync(CancellationToken cancellationToken) => await _busSubscriber.Subscribe(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _busSubscriber.Unsubscribe(cancellationToken);
            return Task.CompletedTask;
        }
    }
}