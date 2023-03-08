using Microsoft.Extensions.DependencyInjection;
using Platform.Tools.Geolocator;
using Platform.Tools.HostGeolocation.Stun;

namespace Platform.Tools.HostGeolocation;

public static class Extensions
{
    public static IServiceCollection AddHostGeolocation(this IServiceCollection services) =>
        services.AddHttpClient()
            .AddScoped<IGeolocator, Ip2CGeolocator>()
            .AddScoped<IHostGeolocation, HostGeolocation>()
            .AddScoped<IStunClient, StunClient>();
}