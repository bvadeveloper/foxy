using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Platform.Contract.Reporter;
using Platform.Contract.Scanner;
using Microsoft.Extensions.Logging;
using Platform.Bus.Publisher.Abstractions;
using Platform.Contract.Abstractions;
using Platform.Logging.Extensions;
using Platform.Tools.Abstractions;
using Platform.Tools.Models;

namespace Platform.Consumer.Scanner.Consumers
{
    public class ScanConsumer : IConsumeAsync<DomainScanProfile>
    {
        private readonly IToolsHolder _toolsHolder;
        private readonly IPublishClient _publishClient;
        private readonly ILogger _logger;

        public ScanConsumer(IToolsHolder toolsHolder, IPublishClient publishClient, ILogger<ScanConsumer> logger)
        {
            _toolsHolder = toolsHolder;
            _publishClient = publishClient;
            _logger = logger;
        }

        public async Task ConsumeAsync(DomainScanProfile profile, CancellationToken cancellationToken = new())
        {
            _logger.Trace($"Run scan tools for target '{profile.Target}'");

            var outputs = await _toolsHolder
                .FilterByTargetMarks(profile.Tags)
                .RunTools(profile.Target);

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

            await _publishClient.Publish(new ReportProfile
            {
                SessionContext = profile.SessionContext,
                Target = profile.Target,
                Reports = reports
            });
        }
    }
}