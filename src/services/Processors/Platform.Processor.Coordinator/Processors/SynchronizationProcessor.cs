using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus.Subscriber;
using Platform.Caching.Abstractions;
using Platform.Contract.Profiles;
using Platform.Logging.Extensions;

namespace Platform.Processor.Coordinator.Processors;

internal class SynchronizationProcessor : IConsumeAsync<SynchronizationProfile>
{
    private readonly ICacheDataService _cacheDataService;
    private readonly ILogger _logger;

    private readonly TimeSpan _ttl = TimeSpan.FromMinutes(2);
    private const string HeartbeatPrefix = "hb";

    public SynchronizationProcessor(ICacheDataService cacheDataService, ILogger<SynchronizationProcessor> logger)
    {
        _cacheDataService = cacheDataService;
        _logger = logger;
    }

    public async ValueTask ConsumeAsync(SynchronizationProfile profile)
    {
        await _cacheDataService.SetValue(profile.Route, profile.HostId, _ttl, true);

        if (profile.Route.LastIndexOf('*') == -1)
        {
            await _cacheDataService.SetValue(MakeKey(profile), profile.HostId, _ttl, true);
            _logger.Trace($"Heartbeat info: host with id '{profile.HostId}' consume following route '{profile.Route}'");
        }

#if DEBUG
        var key = $"{HeartbeatPrefix}:{profile.Route}*";
        var hostList = await _cacheDataService.KeyScan(key, 100);
        _logger.Info($"Host count for route '{profile.Route}' is '{hostList.Count}'");
#endif
    }

    private static string MakeKey(SynchronizationProfile profile) => $"{HeartbeatPrefix}:{profile.Route}:{profile.HostId}";
}