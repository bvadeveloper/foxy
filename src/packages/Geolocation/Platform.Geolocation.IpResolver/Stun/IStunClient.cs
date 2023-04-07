using System.Net;

namespace Platform.Geolocation.IpResolver.Stun;

public interface IStunClient
{
    Task<IPAddress> Send();
}