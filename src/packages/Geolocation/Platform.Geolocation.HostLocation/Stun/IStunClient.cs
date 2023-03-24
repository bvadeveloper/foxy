using System.Net;

namespace Platform.Geolocation.HostLocation.Stun;

public interface IStunClient
{
    Task<IPAddress> Send();
}