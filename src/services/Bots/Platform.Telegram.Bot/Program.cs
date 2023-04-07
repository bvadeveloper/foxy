using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Subscriber;
using Platform.Bus.Subscriber.EventProcessors;
using Platform.Contract.Profiles.Processors;
using Platform.Cryptography;
using Platform.Host;
using Platform.Limiter.Redis;
using Platform.Telegram.Bot.Clients;
using Platform.Telegram.Bot.Extensions;
using Platform.Telegram.Bot.Parsers;
using Platform.Validation.Fluent;

namespace Platform.Telegram.Bot;

internal static class Program
{
    public static async Task Main(string[] args) =>
        await Application.RunAsync(args, (services, configuration) =>
        {
            services
                
                // bus
                .AddSubscriptions(configuration, ExchangeNames.Telegram)
                .AddScoped<IEventProcessor, EventProcessor>()
                
                // services
                .AddTelegram(configuration)
                .AddRequestLimiter(configuration)
                .AddValidation()
                .AddMockCryptographicServices()
                .AddSingleton<IMessageParser, SimpleMessageParser>()
                
                // processors
                .AddScoped<IConsumeAsync<ReportProfile>, ResponderProcessor>()
                .AddScoped<ICoordinatorClient, CoordinatorClient>();;
        });
}