using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Caching.Abstractions;
using Platform.Contract.Profiles;
using Platform.Geolocation.HostGeolocation;

namespace Platform.Processor.Coordinator.Strategies;

public class HostProcessingStrategy : IProcessingStrategy
{
    private readonly IHostGeolocation _targetGeolocation;
    private readonly ICacheDataService _cacheDataService;
    private readonly IBusPublisher _publisher;
    private readonly ILogger _logger;

    public HostProcessingStrategy(IBusPublisher publisher, ICacheDataService cacheDataService, IHostGeolocation targetGeolocation, ILogger logger)
    {
        _publisher = publisher;
        _cacheDataService = cacheDataService;
        _targetGeolocation = targetGeolocation;
        _logger = logger;
    }

    public ProcessingTypes ProcessingType { get; init; } = ProcessingTypes.Host;

    public async Task Run(CoordinatorProfile profile)
    {
        // var ipAddress = IPAddress.Parse(profile.TargetNames);
        // var location = await _targetGeolocation.FindGeoMarkers(ipAddress);
        // var route = await MakeRoute(location, ExchangeTypes.Host);
        //
        // var hostProfile = new HostProfile(profile.TargetNames, new IpLocation(location, ipAddress.ToString()));
        // await _publisher.PublishToHostExchange(hostProfile, route);
    }

    private async Task<string> MakeRoute(string location, ExchangeTypes exchangeType)
    {
        // var locationRoute = exchangeType.ToLocationRoute(location);
        //
        // // let's check available scanners by location
        // if (await _cacheDataService.KeyExists(locationRoute))
        // {
        //     return locationRoute;
        // }
        //
        // var defaultRoute = exchangeType.ToDefaultRoute();
        //
        // // let's check any scanners from the pool
        // if (await _cacheDataService.KeyExists(defaultRoute))
        // {
        //     return defaultRoute;
        // }

        throw new NotImplementedException();
    }
}