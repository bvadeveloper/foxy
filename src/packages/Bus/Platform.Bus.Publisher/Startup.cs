using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus.EasyNetQ;
using Platform.Bus.EasyNetQ.Configurations;
using Platform.Bus.Publisher.Abstractions;

namespace Platform.Bus.Publisher;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<BusConfiguration>(options =>
            Configuration.GetSection("Bus").Bind(options));

         services
            .AddScoped<IPublishClient, PublishClient>()
            .AddBus();
    }
}