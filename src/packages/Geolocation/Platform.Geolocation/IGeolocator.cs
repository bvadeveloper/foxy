using System.Net;

namespace Platform.Geolocation;

public interface IGeolocator
{
    /// <summary>
    /// In our case, we need only the country code, two-letter (ISO 3166)
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <returns></returns>
    public ValueTask<string> FindCountryCode(IPAddress ipAddress);
}