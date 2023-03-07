using System.Net;

namespace Platform.Tool.GeoService.Abstractions;

public interface IGeoService
{
    Task<string> FindTargetGeoMarker(IPAddress ipAddress);
    Task<string> FindLocalGeoMarker();
}