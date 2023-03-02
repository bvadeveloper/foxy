using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;

namespace Platform.Consumer.Reporter.Consumers
{
    public class ReportConsumer : IConsumeAsync<Profile>
    {
        private readonly IBusPublisher _publisher;
        private readonly IReportService _reportService;
        private readonly ILogger _logger;

        public ReportConsumer(
            IBusPublisher publisher,
            IReportService reportService,
            ILogger<ReportConsumer> logger)
        {
            _reportService = reportService;
            _publisher = publisher;
            _logger = logger;
        }

        public async ValueTask ConsumeAsync(Profile profile) =>
            await PublishTelegramProfile(profile);


        private async Task PublishTelegramProfile(Profile profile)
        {
            var (fileName, fileBody) = await _reportService.MakeFileReport(profile.TargetName, profile.ToolOutputs);
            profile.FileReport = new FileReport(fileName, fileBody);
            
            await _publisher.PublishToBotExchange(profile);
        }
    }
}