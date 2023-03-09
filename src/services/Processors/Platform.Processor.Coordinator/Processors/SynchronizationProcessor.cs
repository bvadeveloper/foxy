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

    private readonly TimeSpan _ttl = TimeSpan.FromMinutes(10);

    public SynchronizationProcessor(ICacheDataService cacheDataService, ILogger<SynchronizationProcessor> logger)
    {
        _cacheDataService = cacheDataService;
        _logger = logger;
    }

    public async ValueTask ConsumeAsync(SynchronizationProfile profile)
    {
        _logger.Info($"------------->> {profile.Route} -------------> {profile.HostId}");
        await _cacheDataService.SetValue(profile.Route, profile.HostId, _ttl, true);
    }
}