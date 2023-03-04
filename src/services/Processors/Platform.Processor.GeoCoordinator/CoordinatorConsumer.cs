using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;

namespace Platform.Processor.GeoCoordinator;

public class CoordinatorConsumer : IConsumeAsync<Profile>
{
    private readonly IBusPublisher _publisher;
    private readonly ILogger _logger;

    public CoordinatorConsumer(IBusPublisher publisher, ILogger<CoordinatorConsumer> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async ValueTask ConsumeAsync(Profile profile)
    {
        await _publisher.PublishToDomainExchange(profile);
    }
}