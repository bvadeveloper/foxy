using Microsoft.Extensions.DependencyInjection;
using Platform.Geolocation.Ip2c;
using Platform.Geolocation.Maxmind;

namespace Platform.Geolocation.HostGeolocation;

public static class Extensions
{
    public static IServiceCollection AddHostGeolocation(this IServiceCollection services) =>
        services.AddHttpClient()
            .AddScoped<IGeolocator, MaxmindGeolocator>() // in this place you can specify the priority for searching sources
            .AddScoped<IGeolocator, Ip2CGeolocator>()
            .AddScoped<IHostGeolocation, HostGeolocation>();
}