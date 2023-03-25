using Microsoft.Extensions.DependencyInjection;

namespace Platform.Cryptography;

public static class Extensions
{
    public static IServiceCollection AddCryptographicServices(this IServiceCollection services) =>
        services
            .AddSingleton<DiffieHellmanKeyMaker>()
            .AddScoped<ICryptographicService, AesCryptographicService>();
}