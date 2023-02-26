using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus.Publisher;
using Platform.Contract.Abstractions;
using Platform.Contract.Reporter;
using Platform.Contract.Scanner;
using Platform.Logging.Extensions;
using Platform.Tools.Abstractions;
using Platform.Tools.Models;

namespace Platform.Consumer.Scanner.Consumers
{
    public class ScanConsumer // : IConsumeAsync<DomainScanProfile>
    {
        private readonly IToolsHolder _toolsHolder;
        private readonly IPublisher _publishClient;
        private readonly ILogger _logger;

        public ScanConsumer(IToolsHolder toolsHolder, IPublisher publishClient, ILogger<ScanConsumer> logger)
        {
            _toolsHolder = toolsHolder;
            _publishClient = publishClient;
            _logger = logger;
        }

        public async Task ConsumeAsync(DomainScanProfile profile, CancellationToken cancellationToken = new())
        {
            _logger.Trace($"Run scan tools for target '{profile.Name}'");

            var outputs = await _toolsHolder
                .FilterByTargetMarks(profile.Tags)
                .RunTools(profile.Name);

            await PublishReportProfile(profile, outputs);
        }

        private async Task PublishReportProfile(ITarget profile, IEnumerable<OutputModel> outputs)
        {
            var reports = outputs
                .Where(model => model.Successful)
                .Select(report => new ReportModel
                {
                    ToolName = report.ToolName,
                    Output = report.Output,
                    ProcessingDate = DateTime.UtcNow
                })
                .ToArray();

            // await _publishClient.Publish(new ReportProfile
            // {
            //     SessionContext = profile.SessionContext,
            //     Value = profile.Value,
            //     Reports = reports
            // });
        }
    }
}