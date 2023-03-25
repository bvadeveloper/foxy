using Microsoft.Extensions.DependencyInjection;
using Platform.Geolocation.IpResolver.Stun;

namespace Platform.Geolocation.IpResolver;

public static class Extensions
{
    public static IServiceCollection AddPublicIpResolver(this IServiceCollection services) =>
        services.AddHttpClient()
            .AddScoped<IPublicIpResolver, PublicIpResolver>()
            .AddScoped<IStunClient, StunClient>();
}