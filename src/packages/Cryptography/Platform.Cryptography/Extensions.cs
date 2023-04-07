using Microsoft.Extensions.DependencyInjection;

namespace Platform.Cryptography;

public static class BootstrapExtensions
{
    public static IServiceCollection AddAesCryptographicServices(this IServiceCollection services) =>
        services
            .AddSingleton<ICryptographicService, AesCryptographicService>();

    public static IServiceCollection AddMockCryptographicServices(this IServiceCollection services) =>
        services
            .AddSingleton<ICryptographicService, MockCryptographicService>();
}

public static class Extensions
{
    public static string ToBase64(this byte[] value) => Convert.ToBase64String(value, Base64FormattingOptions.None);

    public static byte[] ToBytesFromBase64(this string base64String)
    {
        Span<byte> buffer = stackalloc byte[base64String.Length];
        Convert.TryFromBase64String(base64String, buffer, out _);
        return buffer.ToArray();
    }
}