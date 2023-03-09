using System.Net;
using Microsoft.Extensions.Logging;
using Platform.Caching.Abstractions;
using Platform.Geolocation;
using static System.Text.Encoding;

namespace Platform.Geolocation.TargetGeolocation;

public class TargetGeolocation : ITargetGeolocation
{
    private readonly IEnumerable<IGeolocator> _geolocators;
    private readonly ICacheDataService _cacheDataService;
    private readonly ILogger _logger;

    private readonly TimeSpan _ttl = TimeSpan.FromDays(1);

    public TargetGeolocation(IEnumerable<IGeolocator> geolocators, ICacheDataService cacheDataService, ILogger<TargetGeolocation> logger)
    {
        _geolocators = geolocators;
        _cacheDataService = cacheDataService;
        _logger = logger;
    }

    public async Task<string> FindGeoMarkers(IPAddress ipAddress)
    {
        var key = UTF8.GetString(ipAddress.GetAddressBytes());
        var countryCode = await _cacheDataService.GetValue(key);

        if (string.IsNullOrEmpty(countryCode))
        {
            countryCode = await _geolocators.Find(ipAddress);
            await _cacheDataService.SetValue(key, countryCode, _ttl, true);
        }

        return countryCode;
    }
}