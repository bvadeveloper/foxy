using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Platform.Bus;

namespace Platform.Telegram.Bot;

public class SubscriptionService : IHostedService
{
    private readonly IBusSubscriber _busSubscriber;

    public SubscriptionService(IBusSubscriber busSubscriber) => _busSubscriber = busSubscriber;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _busSubscriber.Subscribe(cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _busSubscriber.Unsubscribe(cancellationToken);
        return Task.CompletedTask;
    }
}