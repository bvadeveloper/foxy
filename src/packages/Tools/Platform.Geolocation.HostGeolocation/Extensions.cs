using Microsoft.Extensions.DependencyInjection;
using Platform.Geolocation.HostGeolocation.Stun;
using Platform.Geolocation.Ip2c;
using Platform.Geolocation.Maxmind;

namespace Platform.Geolocation.HostGeolocation;

public static class Extensions
{
    public static IServiceCollection AddHostGeolocation(this IServiceCollection services) =>
        services.AddHttpClient()
            .AddScoped<IGeolocator, MaxmindGeolocator>()
            .AddScoped<IGeolocator, Ip2CGeolocator>()
            .AddScoped<IHostGeolocation, HostGeolocation>()
            .AddScoped<IStunClient, StunClient>();
}