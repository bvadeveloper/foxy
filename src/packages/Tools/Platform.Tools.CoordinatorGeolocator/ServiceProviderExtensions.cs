using Microsoft.Extensions.DependencyInjection;
using Platform.Tools.Geolocator;

namespace Platform.Tools.CoordinatorGeolocator;

public static class ServiceProviderExtensions
{
    public static IServiceCollection AddCoordinatorGeolocator(this IServiceCollection services) =>
        services.AddHttpClient()
            .AddScoped<IGeolocator, Ip2CGeolocator>()
            .AddScoped<ICoordinatorGeolocator, CoordinatorGeolocator>();
}