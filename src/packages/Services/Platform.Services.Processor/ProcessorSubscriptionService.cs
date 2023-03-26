using Microsoft.Extensions.Hosting;
using Platform.Bus.Abstractions;

namespace Platform.Services.Processor;

public class ProcessorSubscriptionService : IHostedService
{
    private readonly IBusSubscriber _busSubscriber;

    public ProcessorSubscriptionService(IBusSubscriber busSubscriber) => _busSubscriber = busSubscriber;

    public async Task StartAsync(CancellationToken cancellationToken) => await _busSubscriber.Subscribe(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _busSubscriber.Unsubscribe(cancellationToken);
        return Task.CompletedTask;
    }
}