using System.Net;

namespace Platform.Geolocation.TargetGeolocation;

public interface ITargetGeolocation
{
    /// <summary>
    /// Find geo markers for targets
    /// </summary>
    /// <returns></returns>
    Task<string> FindGeoMarkers(IPAddress ipAddress);
}