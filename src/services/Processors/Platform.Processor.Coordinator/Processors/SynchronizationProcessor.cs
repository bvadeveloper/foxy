using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus.Subscriber;
using Platform.Caching.Abstractions;
using Platform.Contract.Profiles;
using Platform.Cryptography;
using Platform.Geolocation.HostGeolocation;
using Platform.Logging.Extensions;

namespace Platform.Processor.Coordinator.Processors;

internal class SynchronizationProcessor : IConsumeAsync<SynchronizationProfile>
{
    private readonly ICacheDataService _cacheDataService;
    private readonly DiffieHellmanKeyGenerator _keyGenerator;
    private readonly IHostGeolocation _hostGeolocation;
    private readonly ILogger _logger;

    private readonly TimeSpan _ttl = TimeSpan.FromMinutes(5);

    public SynchronizationProcessor(ICacheDataService cacheDataService, DiffieHellmanKeyGenerator keyGenerator, IHostGeolocation hostGeolocation,
        ILogger<SynchronizationProcessor> logger)
    {
        _cacheDataService = cacheDataService;
        _keyGenerator = keyGenerator;
        _hostGeolocation = hostGeolocation;
        _logger = logger;
    }

    public async ValueTask ConsumeAsync(SynchronizationProfile profile)
    {
        try
        {
            var location = await _hostGeolocation.FindGeolocation(new IPAddress(profile.IpAddress));
            var cacheKey = MakeKey(profile.CollectorInfo, location);

            if (!await _cacheDataService.KeyExists(cacheKey))
            {
                var derivedSecret = _keyGenerator.MakeDerivedSecret(profile.PublicKey);
                await _cacheDataService.SetValue(cacheKey, derivedSecret, _ttl, true);

#if DEBUG
                await _cacheDataService.SetValue(profile.CollectorInfo.Identifier, profile.CollectorInfo.Version, _ttl, true);
#endif
            }
        }
        catch (Exception e)
        {
            _logger.Error($"I see an error in the collector synchronization flow '{e.Message}'", e);
        }
    }

    private static string MakeKey(CollectorInfo collectorInfo, string location) => $"{location}:{collectorInfo}";
}