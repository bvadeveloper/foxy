using System.Net;

namespace Platform.Tools.CoordinatorGeolocator;

public interface ICoordinatorGeolocator
{
    /// <summary>
    /// Find geo markers for targets
    /// </summary>
    /// <returns></returns>
    Task<string> FindGeoMarkers(IPAddress ipAddress);
}