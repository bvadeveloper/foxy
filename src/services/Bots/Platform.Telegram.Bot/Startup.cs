using Microsoft.AspNetCore.Builder;
using Platform.Telegram.Bot.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;

namespace Platform.Telegram.Bot
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) =>
            services
                .AddTelegramBot(Configuration)
                .AddExchangeListeners(ExchangeTypes.Telegram)
                .AddScoped<IConsumeAsync<Profile>, ResponderProcessor>();

        public void Configure(IApplicationBuilder app) => app.UseHealthChecks("/status");
    }
}