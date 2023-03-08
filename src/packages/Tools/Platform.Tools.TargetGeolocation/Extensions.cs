using Microsoft.Extensions.DependencyInjection;
using Platform.Tools.Geolocator;

namespace Platform.Tools.TargetGeolocation;

public static class Extensions
{
    public static IServiceCollection AddTargetGeolocation(this IServiceCollection services) =>
        services.AddHttpClient()
            .AddScoped<IGeolocator, Ip2CGeolocator>()
            .AddScoped<ITargetGeolocation, TargetGeolocation>();
}