using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus.Rmq;

namespace Platform.Bus.Publisher;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services) =>
        services
            .AddRmqConfiguration(Configuration)
            .AddScoped<IPublisher, Publisher>()
            .AddRmq();
}