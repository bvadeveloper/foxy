using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Bus.Abstractions;
using Platform.Caching.Abstractions;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Enums;
using Platform.Geolocation.HostGeolocation;

namespace Platform.Processor.Coordinator.Strategies;

public class HostProcessingStrategy : IProcessingStrategy
{
    private readonly IHostGeolocation _geolocation;
    private readonly ICacheDataService _cacheDataService;
    private readonly IBusPublisher _publisher;
    private readonly ILogger _logger;

    public HostProcessingStrategy(
        IBusPublisher publisher,
        ICacheDataService cacheDataService,
        IHostGeolocation geolocation,
        ILogger logger)
    {
        _publisher = publisher;
        _cacheDataService = cacheDataService;
        _geolocation = geolocation;
        _logger = logger;
    }

    public ProcessingTypes ProcessingType { get; init; } = ProcessingTypes.Host;

    public async Task Run(CoordinatorProfile profile)
    {
        var ipAddress = IPAddress.Parse(profile.TargetNames);
        // var location = await _targetGeolocation.FindGeoMarkers(ipAddress);
        // var route = await MakeRoute(location, ExchangeTypes.Host);
        //
        // var hostProfile = new HostProfile(profile.TargetNames, new IpLocation(location, ipAddress.ToString()));
        // await _publisher.PublishToHostExchange(hostProfile, route);
    }
}