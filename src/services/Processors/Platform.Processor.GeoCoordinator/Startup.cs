using Microsoft.Extensions.DependencyInjection;
using Platform.Bus.Subscriber;
using Platform.Contract.Abstractions;

namespace Platform.Processor.GeoCoordinator
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) =>
            services.AddExchanges(typeof(IBotExchange));
    }
}