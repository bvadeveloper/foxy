using Microsoft.Extensions.DependencyInjection;
using Platform.Geolocation.HostLocation.Stun;

namespace Platform.Geolocation.HostLocation;

public static class Extensions
{
    public static IServiceCollection AddHostLocationServices(this IServiceCollection services) =>
        services.AddHttpClient()
            .AddScoped<IHostLocation, HostLocation>()
            .AddScoped<IStunClient, StunClient>();
}