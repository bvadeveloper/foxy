using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Extensions;
using Platform.Cryptography;

namespace Platform.Services.Collector;

public class CollectorPublisher : ICollectorPublisher
{
    private readonly IBusPublisher _publishClient;
    private readonly PublicKeyHolder _keyHolder;
    private readonly ILogger _logger;

    public CollectorPublisher(IBusPublisher publishClient, PublicKeyHolder keyHolder, ILogger<CollectorPublisher> logger)
    {
        _publishClient = publishClient;
        _keyHolder = keyHolder;
        _logger = logger;
    }

    public ValueTask PublishToReport(IProfile profile) => _publishClient.Publish(profile.ToBytes(), Exchange.Default(ExchangeNames.Report), _keyHolder.Value);
}