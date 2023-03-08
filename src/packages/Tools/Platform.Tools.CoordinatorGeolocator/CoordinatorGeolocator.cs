using System.Net;
using Microsoft.Extensions.Logging;
using Platform.Caching.Abstractions;
using Platform.Tools.Geolocator;

namespace Platform.Tools.CoordinatorGeolocator;

public class CoordinatorGeolocator : ICoordinatorGeolocator
{
    private readonly IGeolocator _geolocator;
    private readonly ICacheDataService _cacheDataService;
    private readonly ILogger _logger;

    public CoordinatorGeolocator(IGeolocator geolocator, ICacheDataService cacheDataService, ILogger<CoordinatorGeolocator> logger)
    {
        _geolocator = geolocator;
        _cacheDataService = cacheDataService;
        _logger = logger;
    }

    public async Task<string> FindGeoMarkers(IPAddress ipAddress)
    {
        return await _geolocator.Find(ipAddress);
    }
}