using System.Net;
using Microsoft.Extensions.Logging;
using Platform.Caching.Abstractions;
using Platform.Tools.Geolocator;
using static System.Text.Encoding;

namespace Platform.Tools.TargetGeolocation;

public class TargetGeolocation : ITargetGeolocation
{
    private readonly IGeolocator _geolocator;
    private readonly ICacheDataService _cacheDataService;
    private readonly ILogger _logger;

    private readonly TimeSpan _ttl = TimeSpan.FromDays(1);

    public TargetGeolocation(IGeolocator geolocator, ICacheDataService cacheDataService, ILogger<TargetGeolocation> logger)
    {
        _geolocator = geolocator;
        _cacheDataService = cacheDataService;
        _logger = logger;
    }

    public async Task<string> FindGeoMarkers(IPAddress ipAddress)
    {
        var key = UTF8.GetString(ipAddress.GetAddressBytes());
        var countryCode = await _cacheDataService.GetValue(key);

        if (string.IsNullOrEmpty(countryCode))
        {
            countryCode = await _geolocator.FindCountryCode(ipAddress);
            await _cacheDataService.SetValue(key, countryCode, _ttl, true);
        }

        return countryCode;
    }
}