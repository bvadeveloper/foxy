using System.Net;

namespace Platform.Geolocation.HostGeolocation.Stun;

public interface IStunClient
{
    Task<IPAddress> Send();
}