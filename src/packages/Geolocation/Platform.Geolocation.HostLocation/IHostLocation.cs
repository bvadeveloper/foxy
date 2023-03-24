using System.Net;

namespace Platform.Geolocation.HostLocation;

/// <summary>
/// Resolve host public IP address
/// </summary>
public interface IHostLocation
{
    /// <summary>
    /// Find public IP address
    /// </summary>
    /// <returns></returns>
    Task<IPAddress> FindPublicIpAddress();
}