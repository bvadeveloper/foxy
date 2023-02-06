using EasyNetQ.AutoSubscribe;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus.EasyNetQ;
using Platform.Bus.EasyNetQ.Configurations;
using Platform.Bus.Subscriber.Abstractions;

namespace Platform.Bus.Subscriber;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<BusConfiguration>(options =>
            Configuration.GetSection("Bus").Bind(options));

        services
            //.AddHostedService<BusHostedService>()
            //.AddScoped<IAutoSubscriberMessageDispatcher, MessageDispatcher>()
            // .AddScoped<IBusSubscriber, BusSubscriber>()
            .AddBus();
    }
}