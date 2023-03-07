using System.Net;
using Microsoft.Extensions.Logging;
using Platform.Tool.GeoService.Abstractions;
using Platform.Tool.GeoService.Stunt;

namespace Platform.Tool.GeoService;

public class GeoService : IGeoService
{
    private readonly IStuntClient _stuntClient;
    private readonly ILogger _logger;
    
    public GeoService(IStuntClient stuntClient, ILogger<GeoService> logger)
    {
        _stuntClient = stuntClient;
        _logger = logger;
    }

    public Task<string> FindTargetGeoMarker(IPAddress ipAddress)
    {
        throw new NotImplementedException();
    }

    public async Task<string> FindLocalGeoMarker()
    {
        var externalIp = await _stuntClient.RequestExternalIp();
        
        // call geoip resolver
        
        return "AU";
    }
}