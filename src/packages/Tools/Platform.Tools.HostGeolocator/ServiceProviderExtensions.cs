using Microsoft.Extensions.DependencyInjection;
using Platform.Tools.Geolocator;
using Platform.Tools.HostGeolocator.Stun;

namespace Platform.Tools.HostGeolocator;

public static class ServiceProviderExtensions
{
    public static IServiceCollection AddHostGeolocator(this IServiceCollection services) =>
        services.AddHttpClient()
            .AddScoped<IGeolocator, Ip2CGeolocator>()
            .AddScoped<IHostGeolocator, HostGeolocator>()
            .AddScoped<IStunClient, StunClient>();
}