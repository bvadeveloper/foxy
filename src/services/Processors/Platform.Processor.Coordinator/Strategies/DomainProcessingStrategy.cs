using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Contract.Profiles.Collectors;
using Platform.Contract.Profiles.Enums;
using Platform.Contract.Profiles.Processors;
using Platform.Geolocation.HostGeolocation;
using Platform.Processor.Coordinator.Clients;
using Platform.Tool.Cloudflare;

namespace Platform.Processor.Coordinator.Strategies;

public class DomainProcessingStrategy : IProcessingStrategy
{
    private readonly ICloudflareResolver _cloudflareResolver;
    private readonly ICollectorClient _processorClient;
    private readonly IHostGeolocation _geolocation;
    private readonly ILogger _logger;

    public DomainProcessingStrategy(
        IHostGeolocation geolocation,
        ICollectorClient processorClient,
        ICloudflareResolver cloudflareResolver,
        ILogger<DomainProcessingStrategy> logger)
    {
        _processorClient = processorClient;
        _geolocation = geolocation;
        _cloudflareResolver = cloudflareResolver;
        _logger = logger;
    }

    public ProcessingTypes ProcessingType { get; init; } = ProcessingTypes.Domain;

    public async Task Run(CoordinatorProfile profile)
    {
        var domainName = profile.Target;

        // check if domain has cloudflare protection
        var hasProtection = await _cloudflareResolver.HasProtection(domainName);

        IPAddress[] ipAddresses;

        if (hasProtection)
        {
            ipAddresses = await _cloudflareResolver.TryResolveRealIpAddresses(domainName);
        }
        else
        {
            ipAddresses = GetDomainIpAddress(domainName);
        }

        var ipLocations = await GetIpLocations(ipAddresses);
        var domainProtection = hasProtection
            ? new DomainProtection(DomainProtectionTypes.Cloudflare)
            : new DomainProtection(DomainProtectionTypes.NotFound);

        var domainProfile = new DomainProfile(profile.Target, ProcessingType, ipLocations, domainProtection);


        await _processorClient.SendToDomainScanner(domainProfile);
    }

    private async Task<ImmutableArray<IpLocation>> GetIpLocations(IPAddress[] ipAddresses)
    {
        var ipLocations = await Task.WhenAll(ipAddresses.Select(async ipAddress =>
        {
            var countryCode = await _geolocation.FindCountryCode(ipAddress);
            return new IpLocation(countryCode, ipAddress.ToString());
        }));

        return ipLocations.ToImmutableArray();
    }

    private static IPAddress[] GetDomainIpAddress(string domainName) => Dns.GetHostAddresses(domainName);
}