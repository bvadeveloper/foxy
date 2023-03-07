using System.Net;

namespace Platform.Tool.GeoService.Stunt;

public interface IStuntClient
{
    Task<IPAddress> RequestExternalIp();
}