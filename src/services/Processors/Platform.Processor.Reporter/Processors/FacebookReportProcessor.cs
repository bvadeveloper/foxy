using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;

namespace Platform.Processor.Reporter.Processors
{
    public class FacebookReportProcessor : IConsumeAsync<FacebookProfile>
    {
        private readonly IBusPublisher _publisher;
        private readonly IReportBuilder _reportBuilder;
        private readonly ILogger _logger;

        public FacebookReportProcessor(
            IBusPublisher publisher,
            IReportBuilder reportBuilder,
            ILogger<DomainReportProcessor> logger)
        {
            _reportBuilder = reportBuilder;
            _publisher = publisher;
            _logger = logger;
        }

        public async ValueTask ConsumeAsync(FacebookProfile profile) =>
            await PublishTelegramProfile(profile);


        private async Task PublishTelegramProfile(FacebookProfile profile)
        {
            var domainReport = new ReportProfile(profile.TargetName);
            var (fileName, fileBody) = await _reportBuilder.BuildTextFileReport(profile.TargetName, profile.ToolOutputs);
            domainReport.FileReport = new FileReport(fileName, fileBody);

            await _publisher.PublishToTelegramExchange(domainReport);
        }
    }
}