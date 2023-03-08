namespace Platform.Tools.HostGeolocator;

public interface IHostGeolocator
{
    /// <summary>
    /// Find geo markers for scanner applications
    /// </summary>
    /// <returns></returns>
    Task<string> FindGeoMarkers();
}