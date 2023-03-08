using System.Net;

namespace Platform.Tools.Geolocator;

public interface IGeolocator
{
    /// <summary>
    /// In our case, we need only the country code, two-letter (ISO 3166)
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <returns></returns>
    public Task<string> FindCountryCode(IPAddress ipAddress);
}