using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Platform.Consumer.Reporter.Abstractions;
using Platform.Contract.Reporter;
using Platform.Contract.Reporter.Abstractions;
using Platform.Contract.Telegram;
using Microsoft.Extensions.Logging;
using Platform.Bus.Publisher.Abstractions;

namespace Platform.Consumer.Reporter.Consumers
{
    public class ReportConsumer : IConsumeAsync<ReportProfile>
    {
        private readonly IPublishClient _publishClient;
        private readonly IReportService _reportService;
        private readonly ILogger _logger;

        public ReportConsumer(
            IPublishClient publishClient,
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
            var (fileName, fileBody) = await _reportService.MakeFileReport(profile.Value, profile.Reports);
            await _publishClient.Publish(new TelegramProfile
            {
                SessionContext = profile.SessionContext,
                Value = profile.Value,
                FileBody = fileBody,
                FileName = fileName
            });
        }
    }
}