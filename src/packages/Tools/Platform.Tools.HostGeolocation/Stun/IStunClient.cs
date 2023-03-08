using System.Net;

namespace Platform.Tools.HostGeolocation.Stun;

public interface IStunClient
{
    Task<IPAddress> Send();
}