using Microsoft.Extensions.DependencyInjection;

namespace Platform.Cryptography;

public static class BootstrapExtensions
{
    public static IServiceCollection AddCryptographicServices(this IServiceCollection services) =>
        services
            .AddSingleton<ICryptographicService, AesCryptographicService>();
}

public static class Extensions
{
    public static string ToBase64String(this byte[] value) => Convert.ToBase64String(value, Base64FormattingOptions.None);
}