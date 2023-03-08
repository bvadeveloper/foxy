using System.Net;

namespace Platform.Tools.HostGeolocator.Stun;

public interface IStunClient
{
    Task<IPAddress> Send();
}