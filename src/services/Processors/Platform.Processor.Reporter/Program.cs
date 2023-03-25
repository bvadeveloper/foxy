using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Platform.Host;
using Platform.Processor.Reporter.Processors;
using Platform.Services.Background;

namespace Platform.Processor.Reporter;

internal static class Program
{
    public static async Task Main(string[] args) =>
        await Application.RunAsync(args, (services, configuration) =>
        {
            services
                .AddPublisher(configuration)
                .AddProcessorSubscriber(configuration)
                .AddExchangeListeners(ExchangeTypes.ReportExchange)
                .AddScoped<IReportBuilder, ReportBuilder>()
                
                // report processors
                .AddScoped<IConsumeAsync<DomainProfile>, DomainReportProcessor>()
                .AddScoped<IConsumeAsync<HostProfile>, HostReportProcessor>()
                .AddScoped<IConsumeAsync<EmailProfile>, EmailReportProcessor>()
                .AddScoped<IConsumeAsync<FacebookProfile>, FacebookReportProcessor>();
        });
}