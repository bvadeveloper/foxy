using Microsoft.Extensions.DependencyInjection;
using Platform.Bus.Rmq;
using Platform.Contract.Abstractions;

namespace Platform.Processor.GeoCoordinator
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) =>
            services.AddExchangeTypes(typeof(ITelegramExchange));
    }
}