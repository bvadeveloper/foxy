using Microsoft.Extensions.DependencyInjection;

namespace Platform.Serializer.MemoryPack;

public class Startup
{
    public void ConfigureServices(IServiceCollection services) =>
        services
            .AddSingleton<ISerializer, BytesSerializer>()
            .AddSingleton<IDeserializer, BytesDeserializer>();
}