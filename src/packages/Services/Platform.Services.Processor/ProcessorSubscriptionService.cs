using Microsoft.Extensions.Hosting;
using Platform.Bus;

namespace Platform.Services.Processor;

public class ProcessorSubscriptionService : IHostedService
{
    private readonly IBusSubscriber _busSubscriber;

    public ProcessorSubscriptionService(IBusSubscriber busSubscriber) => _busSubscriber = busSubscriber;

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