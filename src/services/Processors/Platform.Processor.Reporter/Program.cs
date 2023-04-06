using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Subscriber;
using Platform.Bus.Subscriber.EventProcessors;
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

                // bus
                .AddSubscriptions(configuration, ExchangeNames.Report)
                .AddScoped<IEventProcessor, EventDecryptProcessor>()

                // services
                .AddAesCryptographicServices()
                .AddHostedService<CryptographicKeySynchronizationService>()
                .AddScoped<IReportBuilder, ReportBuilder>()
                
                .AddScoped<PublicKeyHolder>() // this type no needed in reporter

                // processors
                .AddScoped<IConsumeAsync<DomainProfile>, DomainReportProcessor>()
                .AddScoped<IConsumeAsync<HostProfile>, HostReportProcessor>()
                .AddScoped<IConsumeAsync<EmailProfile>, EmailReportProcessor>()
                .AddScoped<IConsumeAsync<FacebookProfile>, FacebookReportProcessor>();
        });
}