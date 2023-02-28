using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Subscriber;

namespace Platform.Processor.GeoCoordinator
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) =>
            services.AddExchange(ExchangeTypes.Telegram);
    }
}