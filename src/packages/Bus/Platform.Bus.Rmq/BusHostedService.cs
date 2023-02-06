using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Platform.Bus.Rmq.Abstractions;

namespace Platform.Bus.Rmq
{
    public class BusHostedService : IHostedService
    {
        private readonly IBusSubscriber _busSubscriber;

        public BusHostedService(IBusSubscriber busSubscriber) => _busSubscriber = busSubscriber;

        public async Task StartAsync(CancellationToken cancellationToken) => await _busSubscriber.Subscribe(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _busSubscriber.Unsubscribe();
            return Task.CompletedTask;
        }
    }
}