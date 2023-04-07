using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Contract.Profiles.Collectors;
using Platform.Contract.Profiles.Enums;
using Platform.Contract.Profiles.Processors;
using Platform.Geolocation.HostGeolocation;
using Platform.Processor.Coordinator.Clients;
using Platform.Services.Processor;

namespace Platform.Processor.Coordinator.Strategies;

public class DomainProcessingStrategy : IProcessingStrategy
{
    private readonly IProcessorClient _processorClient;
    private readonly IHostGeolocation _geolocation;
    private readonly ILogger _logger;

    public DomainProcessingStrategy(IHostGeolocation geolocation, IProcessorClient processorClient, ILogger<DomainProcessingStrategy> logger)
    {
        _processorClient = processorClient;
        _geolocation = geolocation;
        _logger = logger;
    }

    public ProcessingTypes ProcessingType { get; init; } = ProcessingTypes.Domain;

    public async Task Run(CoordinatorProfile profile)
    {
        var countryCode = await _geolocation.FindCountryCode(IPAddress.Parse(profile.Target));
        var ipLocation = new IpLocation(countryCode, profile.Target);
        var hostProfile = new DomainProfile(profile.Target, ProcessingType, ipLocation);

        await _processorClient.SendToDomainScanner(hostProfile);
    }
}