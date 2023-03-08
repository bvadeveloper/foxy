using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;

namespace Platform.Processor.Coordinator;

public class CoordinatorProcessor : IConsumeAsync<Profile>
{
    private readonly IBusPublisher _publisher;
    private readonly ILogger _logger;

    public CoordinatorProcessor(IBusPublisher publisher, ILogger<CoordinatorProcessor> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async ValueTask ConsumeAsync(Profile profile)
    {
        await _publisher.PublishToDomainExchange(profile);
    }
}