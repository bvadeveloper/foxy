using System.Net;

namespace Platform.Tools.TargetGeolocation;

public interface ITargetGeolocation
{
    /// <summary>
    /// Find geo markers for targets
    /// </summary>
    /// <returns></returns>
    Task<string> FindGeoMarkers(IPAddress ipAddress);
}