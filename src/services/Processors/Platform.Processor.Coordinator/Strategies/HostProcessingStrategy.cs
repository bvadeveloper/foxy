using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Enums;
using Platform.Geolocation.HostGeolocation;
using Platform.Logging.Extensions;
using Platform.Processor.Coordinator.Repository;

namespace Platform.Processor.Coordinator.Strategies;

public class HostProcessingStrategy : IProcessingStrategy
{
    private readonly ICollectorInfoRepository _routeResolver;
    private readonly IHostGeolocation _geolocation;
    private readonly IBusPublisher _publisher;
    private readonly ILogger _logger;

    public HostProcessingStrategy(
        ICollectorInfoRepository routeResolver,
        IHostGeolocation geolocation,
        IBusPublisher publisher,
        ILogger<HostProcessingStrategy> logger)
    {
        _routeResolver = routeResolver;
        _geolocation = geolocation;
        _publisher = publisher;
        _logger = logger;
    }

    public ProcessingTypes ProcessingType { get; init; } = ProcessingTypes.Host;

    public async Task Run(CoordinatorProfile coordinatorProfile)
    {
        var countryCode = await _geolocation.FindCountryCode(IPAddress.Parse(coordinatorProfile.TargetNames));
        var (publicKeyBob, route) = await _routeResolver.FindByCountryCode(ProcessingType, countryCode);

        var ipLocation = new IpLocation(countryCode, coordinatorProfile.TargetNames);
        var hostProfile = new HostProfile(coordinatorProfile.TargetNames, ipLocation);

        _logger.Trace($"Publish target '{coordinatorProfile.TargetNames}' to {ProcessingType} collector with route '{route}' and country code '{countryCode}'");

        await _publisher.PublishToHostExchange(hostProfile, route, publicKeyBob);
    }
}