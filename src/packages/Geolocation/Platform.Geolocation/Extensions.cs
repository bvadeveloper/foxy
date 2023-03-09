using System.Net;

namespace Platform.Geolocation;

public static class Extensions
{
    public static async ValueTask<string> Find(this IEnumerable<IGeolocator> geolocators, IPAddress ipAddress)
    {
        var countryCode = string.Empty;

        foreach (var geolocator in geolocators)
        {
            countryCode = await geolocator.FindCountryCode(ipAddress);
            if (!string.IsNullOrWhiteSpace(countryCode)) break;
        }

        return countryCode.ToLower();
    }
}