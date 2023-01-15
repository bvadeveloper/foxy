using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Platform.Contract.Reporter;
using Platform.Contract.Scanner;
using Platform.Tools;
using Platform.Tools.Abstractions;
using Platform.Tools.Models;
using Microsoft.Extensions.Logging;
using Platform.Bus.Publisher.Abstractions;
using Platform.Contract;
using Platform.Contract.Abstractions;
using Platform.Contract.Enums;

namespace Platform.Consumer.Collector.Consumers
{
    public class CollectorConsumer : IConsumeAsync<DomainTarget>
    {
        private readonly IToolsHolder _toolsHolder;
        private readonly IPublishClient _publishClient;
        private readonly ILogger _logger;

        public CollectorConsumer(
            IToolsHolder toolsHolder,
            IPublishClient publishClient,
            ILogger<CollectorConsumer> logger)
        {
            _toolsHolder = toolsHolder;
            _publishClient = publishClient;
            _logger = logger;
        }

        public async Task ConsumeAsync(DomainTarget profile, CancellationToken cancellationToken = new())
        {
            // todo: move to common area

            if (profile.Tools.Any())
            {
                // 1. send scan profile to scanners
                await PublishScanProfile(profile);
            }
            else
            {
                // 1. start collect tools, fill target tags
                var (outputs, tags) = await CollectTargetTags(profile.Target);

                if (tags.ContainsKey(TargetType.NotAvailable))
                {
                    // target is NOT available

                    // 4. send target tags to report host for sending to clients
                    await PublishReportProfile(profile, outputs);
                }
                else
                {
                    // target is available

                    // 2. send scan profile to scanners
                    await PublishScanProfile(profile, tags);

                    // 3. send request to save target tags to DB
                    // await PublishSaveProfile(profile, tags);
                }
            }
        }

        private async Task PublishScanProfile(DomainTarget profile, Dictionary<TargetType, List<string>> tags = default)
        {
            await _publishClient.Publish(new DomainScanProfile
            {
                SessionContext = profile.SessionContext,
                Target = profile.Target,
                Tools = profile.Tools,
                Tags = tags
            });
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

            await _publishClient.Publish(new ReportProfile
            {
                SessionContext = profile.SessionContext,
                Target = profile.Target,
                Reports = reports
            });
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