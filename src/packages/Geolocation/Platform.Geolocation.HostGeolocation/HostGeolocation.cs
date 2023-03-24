using System.Net;
using Microsoft.Extensions.Logging;
using Platform.Caching.Abstractions;
using Platform.Logging.Extensions;

namespace Platform.Geolocation.HostGeolocation;

public class HostGeolocation : IHostGeolocation
{
    private readonly IEnumerable<IGeolocator> _geolocators;
    private readonly ICacheDataService _cacheDataService;
    private readonly ILogger _logger;

    private readonly TimeSpan _ttl = TimeSpan.FromDays(1);
    private const string LocationKey = "geolocation";

    public HostGeolocation(IEnumerable<IGeolocator> geolocators, ICacheDataService cacheDataService, ILogger<HostGeolocation> logger)
    {
        _geolocators = geolocators;
        _cacheDataService = cacheDataService;
        _logger = logger;
    }

    /// <summary>
    /// Build geolocation like 'countryCode_city'
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <returns></returns>
    public async ValueTask<string> FindGeolocation(IPAddress ipAddress)
    {
        var cacheKey = MakeKey(ipAddress);
        var countryCode = await _cacheDataService.GetHashValue(LocationKey, cacheKey);

        if (string.IsNullOrEmpty(countryCode))
        {
            foreach (var geolocator in _geolocators)
            {
                countryCode = (await geolocator.FindCountryCode(ipAddress)).ToLowerInvariant();
                if (!string.IsNullOrWhiteSpace(countryCode)) break;
            }

            await _cacheDataService.SetHashValue(LocationKey, cacheKey, countryCode, true);
            await _cacheDataService.SetExpiry(LocationKey, _ttl, true);
        }

        _logger.Trace($"Mapping '{cacheKey}' -> '{countryCode}'");
        return countryCode;
    }

    private static string MakeKey(IPAddress ipAddress) => ipAddress.ToString();
}