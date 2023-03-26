using System.Threading.Tasks;
using BotMessageParser;
using BotMessageParser.Parsers;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Caching.Redis;
using Platform.Contract.Profiles;
using Platform.Host;
using Platform.Limiter.Redis;
using Platform.Services.Processor;
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
                .AddProcessorSubscriber(configuration)
                .AddExchangeListeners(ExchangeTypes.TelegramExchange)
                .AddRedis(configuration)
                .AddRequestLimiter(configuration)
                .AddValidation()
                .AddSingleton<IMessageParser, CustomMessageParser>()
                .AddScoped<IConsumeAsync<ReportProfile>, ResponderProcessor>();
        });
}