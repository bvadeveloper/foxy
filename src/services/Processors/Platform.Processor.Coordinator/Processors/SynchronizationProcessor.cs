using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus.Subscriber;
using Platform.Caching.Abstractions;
using Platform.Contract.Profiles;
using Platform.Geolocation.HostGeolocation;
using Platform.Logging.Extensions;

namespace Platform.Processor.Coordinator.Processors;

internal class SynchronizationProcessor : IConsumeAsync<SynchronizationProfile>
{
    private readonly ICacheDataService _cacheDataService;
    private readonly IHostGeolocation _hostGeolocation;
    private readonly ILogger _logger;

    private readonly TimeSpan _ttl = TimeSpan.FromMinutes(5);

    public SynchronizationProcessor(ICacheDataService cacheDataService, IHostGeolocation hostGeolocation, ILogger<SynchronizationProcessor> logger)
    {
        _cacheDataService = cacheDataService;
        _hostGeolocation = hostGeolocation;
        _logger = logger;
    }

    public async ValueTask ConsumeAsync(SynchronizationProfile profile)
    {
        try
        {
            _logger.Trace($"Sync request from '{profile.CollectorInfo.ProcessingTypes}' '{profile.CollectorInfo.RouteInfo}'");
            var location = await _hostGeolocation.FindGeolocation(new IPAddress(profile.IpAddress));
            var cacheKey = MakeKey(profile.CollectorInfo, location);

            if (!await _cacheDataService.KeyExists(cacheKey))
            {
                var encodedPublicKey = Convert.ToBase64String(profile.PublicKey, Base64FormattingOptions.None);
                await _cacheDataService.SetValue(cacheKey, encodedPublicKey, _ttl, true);

#if DEBUG
                await _cacheDataService.SetValue(profile.CollectorInfo.RouteInfo, profile.CollectorInfo.Version, _ttl, true);
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