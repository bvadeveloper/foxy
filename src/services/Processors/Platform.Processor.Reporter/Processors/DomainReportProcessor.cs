using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;

namespace Platform.Processor.Reporter.Processors
{
    public class DomainReportProcessor : IConsumeAsync<DomainProfile>
    {
        private readonly IBusPublisher _publisher;
        private readonly IReportBuilder _reportBuilder;
        private readonly ILogger _logger;

        public DomainReportProcessor(
            IBusPublisher publisher,
            IReportBuilder reportBuilder,
            ILogger<DomainReportProcessor> logger)
        {
            _reportBuilder = reportBuilder;
            _publisher = publisher;
            _logger = logger;
        }

        public async Task ConsumeAsync(DomainProfile profile) =>
            await PublishTelegramProfile(profile);


        private async Task PublishTelegramProfile(DomainProfile profile)
        {
            var domainReport = new ReportProfile(profile.TargetName);
            var (fileName, fileBody) = await _reportBuilder.BuildTextFileReport(profile.TargetName, profile.ToolOutputs);
            domainReport.FileReport = new FileReport(fileName, fileBody);

            await _publisher.PublishToTelegramExchange(domainReport);
        }
    }
}