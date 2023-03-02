using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;

namespace Platform.Processor.GeoCoordinator
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) =>
            services.AddExchangeListeners(ExchangeTypes.GeoCoordinator, ExchangeTypes.GeoSynchronization)
                .AddScoped<IConsumeAsync<Profile>, CoordinatorConsumer>();
    }
}