using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Caching.Redis;
using Platform.Contract.Profiles;
using Platform.Cryptography;
using Platform.Host;
using Platform.Processor.Reporter.Processors;
using Platform.Services.Processor;

namespace Platform.Processor.Reporter;

internal static class Program
{
    public static async Task Main(string[] args) =>
        await Application.RunAsync(args, (services, configuration) =>
        {
            services
                .AddPublisher(configuration)
                .AddReporterSubscription(configuration)
                .AddExchanges(ExchangeTypes.Report)
                .AddAesCryptographicServices()
                .AddRedis(configuration)
                .AddScoped<IReportBuilder, ReportBuilder>()
                .AddHostedService<CryptographicKeySynchronizationService>()
                .AddScoped<PublicKeyHolder>() // this type no needed in reporter
                
                // report processors
                .AddScoped<IConsumeAsync<DomainProfile>, DomainReportProcessor>()
                .AddScoped<IConsumeAsync<HostProfile>, HostReportProcessor>()
                .AddScoped<IConsumeAsync<EmailProfile>, EmailReportProcessor>()
                .AddScoped<IConsumeAsync<FacebookProfile>, FacebookReportProcessor>();
        });
}