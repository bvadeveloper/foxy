using System.Threading.Tasks;
using BotMessageParser;
using BotMessageParser.Parsers;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Caching.Redis;
using Platform.Contract.Profiles;
using Platform.Cryptography;
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
                .AddProcessorSubscription(configuration)
                .AddExchanges(ExchangeTypes.Telegram)
                .AddRedis(configuration)
                .AddRequestLimiter(configuration)
                .AddValidation()
                .AddMockCryptographicServices()
                .AddSingleton<IMessageParser, SimpleMessageParser>()
                .AddScoped<IConsumeAsync<ReportProfile>, ResponderProcessor>();
        });
}