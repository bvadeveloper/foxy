using Microsoft.Extensions.DependencyInjection;
using Platform.Geolocation.Ip2c;
using Platform.Geolocation.Maxmind;

namespace Platform.Geolocation.TargetGeolocation;

public static class Extensions
{
    public static IServiceCollection AddTargetGeolocation(this IServiceCollection services) =>
        services.AddHttpClient()
            .AddScoped<IGeolocator, MaxmindGeolocator>() // in this place you can specify the priority for searching sources
            .AddScoped<IGeolocator, Ip2CGeolocator>()
            .AddScoped<ITargetGeolocation, TargetGeolocation>();
}