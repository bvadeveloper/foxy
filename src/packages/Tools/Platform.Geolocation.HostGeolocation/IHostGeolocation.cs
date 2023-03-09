namespace Platform.Geolocation.HostGeolocation;

public interface IHostGeolocation
{
    /// <summary>
    /// Find geo markers for scanner applications
    /// </summary>
    /// <returns></returns>
    Task<string> FindCountryCode();
}