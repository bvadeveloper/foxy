using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;

namespace Platform.Processor.Reporter.Consumers
{
    public class ReportProcessor : IConsumeAsync<Profile>
    {
        private readonly IBusPublisher _publisher;
        private readonly IReportBuilder _reportBuilder;
        private readonly ILogger _logger;

        public ReportProcessor(
            IBusPublisher publisher,
            IReportBuilder reportBuilder,
            ILogger<ReportProcessor> logger)
        {
            _reportBuilder = reportBuilder;
            _publisher = publisher;
            _logger = logger;
        }

        public async ValueTask ConsumeAsync(Profile profile) =>
            await PublishTelegramProfile(profile);


        private async Task PublishTelegramProfile(Profile profile)
        {
            var (fileName, fileBody) = await _reportBuilder.BuildTextFileReport(profile.TargetName, profile.ToolOutputs);
            profile.FileReport = new FileReport(fileName, fileBody);
            
            await _publisher.PublishToTelegramExchange(profile);
        }
    }
}