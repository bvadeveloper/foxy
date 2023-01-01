using Platform.Telegram.Bot.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Telegram.Bot.Consumers;

namespace Platform.Telegram.Bot
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) =>
            services
                .AddScoped<ResponderConsumer>()
                .AddTelegramBot(Configuration);
    }
}