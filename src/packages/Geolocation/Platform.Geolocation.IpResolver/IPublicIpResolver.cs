using System.Net;

namespace Platform.Geolocation.IpResolver;

/// <summary>
/// Resolve host public IP address
/// </summary>
public interface IPublicIpResolver
{
    /// <summary>
    /// Find public IP address
    /// </summary>
    /// <returns></returns>
    Task<IPAddress> FindPublicIpAddress();
}