using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles.Collectors;
using Platform.Contract.Profiles.Processors;
using Platform.Processor.Reporter.Clients;

namespace Platform.Processor.Reporter.Processors
{
    public class DomainReportProcessor : IConsumeAsync<DomainProfile>
    {
        private readonly ITelegramClient _telegramClient;
        private readonly IReportBuilder _reportBuilder;
        private readonly ILogger _logger;

        public DomainReportProcessor(
            ITelegramClient telegramClient,
            IReportBuilder reportBuilder,
            ILogger<DomainReportProcessor> logger)
        {
            _reportBuilder = reportBuilder;
            _telegramClient = telegramClient;
            _logger = logger;
        }

        public async Task ConsumeAsync(DomainProfile profile) =>
            await PublishTelegramProfile(profile);


        private async Task PublishTelegramProfile(DomainProfile profile)
        {
            var domainReport = new ReportProfile(profile.Target);
            var (fileName, fileBody) = await _reportBuilder.BuildTextFileReport(profile.Target, profile.ToolOutputs);
            domainReport.FileReport = new FileReport(fileName, fileBody);

            await _telegramClient.SendToTelegram(domainReport);
        }
    }
}