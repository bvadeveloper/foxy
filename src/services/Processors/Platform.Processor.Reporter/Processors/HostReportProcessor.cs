using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;

namespace Platform.Processor.Reporter.Processors
{
    public class HostReportProcessor : IConsumeAsync<HostProfile>
    {
        private readonly IBusPublisher _publisher;
        private readonly IReportBuilder _reportBuilder;
        private readonly ILogger _logger;

        public HostReportProcessor(
            IBusPublisher publisher,
            IReportBuilder reportBuilder,
            ILogger<HostReportProcessor> logger)
        {
            _reportBuilder = reportBuilder;
            _publisher = publisher;
            _logger = logger;
        }

        public async Task ConsumeAsync(HostProfile profile) =>
            await PublishTelegramProfile(profile);


        private async Task PublishTelegramProfile(HostProfile profile)
        {
            var domainReport = new ReportProfile(profile.TargetName);
            var (fileName, fileBody) = await _reportBuilder.BuildTextFileReport(profile.TargetName, profile.ToolOutputs);
            domainReport.FileReport = new FileReport(fileName, fileBody);

            await _publisher.PublishToTelegramExchange(domainReport);
        }
    }
}