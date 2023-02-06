using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus.Rmq.Abstractions;
using Platform.Bus.Rmq.Configurations;

namespace Platform.Bus.Rmq;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<BusConfiguration>(options =>
            Configuration.GetSection("Bus").Bind(options));

        services
            .AddHostedService<BusHostedService>()
            .AddScoped<IBusSubscriber, BusSubscriber>()
            .AddRmq();
    }
}