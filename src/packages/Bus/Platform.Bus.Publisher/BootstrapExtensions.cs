using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Bus.Publisher;

public static class BootstrapExtensions
{
    public static IServiceCollection AddPublisher(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddBusConfiguration(configuration)
            .AddScoped<IBusPublisher, BusPublisher>()
            .AddBus();
}