using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Caching.Redis;
using Platform.Contract.Profiles;
using Platform.Host;
using Platform.Limiter.Redis;
using Platform.Services;
using Platform.Telegram.Bot.Parser;
using Platform.Validation.Fluent;

namespace Platform.Telegram.Bot;

internal static class Program
{
    public static async Task Main(string[] args) =>
        await Application.RunAsync(args, (services, configuration) =>
        {
            services
                .AddTelegramBot(configuration)
                .AddPublisher(configuration)
                .AddSubscription(configuration)
                .AddExchangeListeners(ExchangeTypes.Telegram)
                .AddRedis(configuration)
                .AddRequestLimiter(configuration)
                .AddValidation()
                .AddSingleton<IMessageParser, MessageParser>()
                .AddScoped<IConsumeAsync<ReportProfile>, ResponderProcessor>();
        });
}