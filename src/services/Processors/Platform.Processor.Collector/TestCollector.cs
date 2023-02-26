using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus.Publisher;
using Platform.Contract.Abstractions;
using Platform.Contract.Enums;
using Platform.Contract.Models.Processor;
using Platform.Contract.Reporter;
using Platform.Tools;
using Platform.Tools.Abstractions;
using Platform.Tools.Models;

namespace Platform.Processor.Collector
{
    public class TestCollector // : IConsumeAsync<TestTarget>
    {
        private readonly IToolsHolder _toolsHolder;
        private readonly IPublisher _publishClient;
        private readonly ILogger _logger;

        public TestCollector(
            IToolsHolder toolsHolder,
            IPublisher publishClient,
            ILogger<TestCollector> logger)
        {
            _toolsHolder = toolsHolder;
            _publishClient = publishClient;
            _logger = logger;
        }

        public async Task ConsumeAsync(TestTarget target, CancellationToken cancellationToken = new())
        {
            if (true)//(target.Tools.Any())
            {
                // 1. send scan profile to scanners
                await PublishScanProfile(target);
            }
            else
            {
                // 1. start collect tools, fill target tags
                var (outputs, tags) = await CollectTargetTags(target.Name);

                if (tags.ContainsKey(TargetType.NotAvailable))
                {
                    // target is NOT available

                    // 4. send target tags to report host for sending to clients
                    await PublishReportProfile(target, outputs);
                }
                else
                {
                    // target is available

                    // 2. send scan profile to scanners
                    await PublishScanProfile(target, tags);

                    // 3. send request to save target tags to DB
                    // await PublishSaveProfile(profile, tags);
                }
            }
        }

        private async Task PublishScanProfile(ITarget profile, Dictionary<TargetType, List<string>> tags = default)
        {
            // await _publishClient.Publish(new DomainScanProfile
            // {
            //     SessionContext = profile.SessionContext,
            //     Value = profile.Value,
            //     //Tools = profile.Tools,
            //     Tags = tags
            // });
        }

        private async Task PublishReportProfile(ITarget profile, IEnumerable<OutputModel> outputs)
        {
            var reports = outputs
                .Where(toolOutput => toolOutput.Successful)
                .Select(toolOutput => new ReportModel
                {
                    ToolName = toolOutput.ToolName,
                    Output = toolOutput.Output,
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

        private async Task<(OutputModel[], Dictionary<TargetType, List<string>>)> CollectTargetTags(string target)
        {
            var outputs = await _toolsHolder.RunTools(target);

            var output = outputs.Aggregate("", (c, m) => $"{c} {m.Output}");
            var dictionary = new Dictionary<TargetType, List<string>>();

            foreach (var mark in (TargetType[])Enum.GetValues(typeof(TargetType)))
            {
                var marks = TargetMarksRepository.FindMarks(mark, output).ToList();
                if (marks.Any()) dictionary.Add(mark, marks);
            }

            return (outputs, dictionary);
        }
    }
}