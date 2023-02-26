using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus.Publisher;
using Platform.Consumer.Reporter.Abstractions;
using Platform.Contract.Reporter;
using Platform.Contract.Reporter.Abstractions;

namespace Platform.Consumer.Reporter.Consumers
{
    public class ReportConsumer // : IConsumeAsync<ReportProfile>
    {
        private readonly IPublisher _publishClient;
        private readonly IReportService _reportService;
        private readonly ILogger _logger;

        public ReportConsumer(
            IPublisher publishClient,
            IReportService reportService,
            ILogger<ReportConsumer> logger)
        {
            _reportService = reportService;
            _logger = logger;
            _publishClient = publishClient;
        }

        public async Task ConsumeAsync(ReportProfile profile, CancellationToken cancellationToken = new()) =>
            await PublishTelegramProfile(profile);

        private async Task PublishTelegramProfile(IReportProfile profile)
        {
            var (fileName, fileBody) = await _reportService.MakeFileReport(profile.Name, profile.Reports);
            // await _publishClient.Publish(new TelegramProfile
            // {
            //     SessionContext = profile.SessionContext,
            //     Value = profile.Value,
            //     FileBody = fileBody,
            //     FileName = fileName
            // });
        }
    }
}