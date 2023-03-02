namespace Platform.Tool.GeoIp;

public interface IGeoIpService
{
    Task<string> GetCurrentLocation();
}