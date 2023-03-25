using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus.Abstractions;

namespace Platform.Bus.Publisher;

public static class BootstrapExtensions
{
    public static IServiceCollection AddPublisher(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddBusConfiguration(configuration)
            .AddScoped<IBusPublisher, Publisher>()
            .AddBus();
}