using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Collectors;
using Platform.Contract.Profiles.Extensions;
using Platform.Contract.Profiles.Processors;
using Platform.Cryptography;
using Platform.Logging.Extensions;

namespace Platform.Services.Collector;

public class CollectorClient : ICollectorClient
{
    private readonly IBusPublisher _publishClient;
    private readonly PublicKeyHolder _keyHolder;
    private readonly ILogger _logger;

    public CollectorClient(IBusPublisher publishClient, PublicKeyHolder keyHolder, ILogger<CollectorClient> logger)
    {
        _publishClient = publishClient;
        _keyHolder = keyHolder;
        _logger = logger;
    }

    /// <summary>
    /// Send collector profiles to reporter exchange
    /// </summary>
    /// <param name="profile"></param>
    /// <returns></returns>
    public async ValueTask SendToReporter(IProfile profile)
    {
        var payload = profile.ToBytes();
        await _publishClient.Publish(payload, Exchange.Default(ExchangeNames.Report), _keyHolder.Value);
        
        _logger.Info($"Sent profile to '{ExchangeNames.Report}' exchange, payload size in mb '{(payload.Length / 1024f) / 1024f}'");
    }

    /// <summary>
    /// Send collector info to sync exchange
    /// </summary>
    /// <param name="collectorInfo"></param>
    /// <param name="ipAddressBytes"></param>
    /// <param name="publicKey"></param>
    public async ValueTask SendToCoordinatorSync(CollectorInfo collectorInfo, byte[] ipAddressBytes, byte[] publicKey)
    {
        var profile = new SynchronizationProfile(collectorInfo, ipAddressBytes, publicKey);
        var payload = profile.ToBytes();
        await _publishClient.Publish(payload, Exchange.Default(ExchangeNames.Sync));
        
        _logger.Info($"Sent sync payload to '{ExchangeNames.Sync}' exchange, payload size in mb '{(payload.Length / 1024f) / 1024f}'");
        
    }
}