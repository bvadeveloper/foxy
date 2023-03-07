using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Tool.GeoService.Abstractions;
using Platform.Tool.GeoService.Stunt;

namespace Platform.Tool.GeoService;

public static class ServiceProviderExtensions
{
    public static IServiceCollection AddGeoServices(this IServiceCollection services, IConfiguration configuration) =>
        services.AddScoped<IGeoService, GeoService>()
            .AddScoped<IStuntClient, StuntClient>();
}