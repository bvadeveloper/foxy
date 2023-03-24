using System.Net;

namespace Platform.Geolocation.HostGeolocation;

public interface IHostGeolocation
{
    /// <summary>
    /// Find geo markers for host
    /// </summary>
    /// <returns></returns>
    ValueTask<string> FindGeolocation(IPAddress ipAddress);
}