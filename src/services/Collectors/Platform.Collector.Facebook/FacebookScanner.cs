using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Platform.Services.Collector;
using Platform.Tools.Abstractions;
using Platform.Tools.Models;

namespace Platform.Collector.Facebook
{
    public class FacebookScanner : IConsumeAsync<FacebookProfile>
    {
        private readonly ICollectorPublisher _collectorPublisher;
        private readonly IToolsHolder _toolsHolder;
        private readonly ILogger _logger;

        public FacebookScanner(IToolsHolder toolsHolder, ICollectorPublisher collectorPublisher, ILogger<FacebookScanner> logger)
        {
            _collectorPublisher = collectorPublisher;
            _toolsHolder = toolsHolder;
            _logger = logger;
        }

        public async Task ConsumeAsync(FacebookProfile profile)
        {
            // var outputs = await _toolsHolder
            //     .FilterByTargetMarks(profile.Tags)
            //     .RunTools(profile.Name);

            await PublishReportProfile(profile, new OutputModel[] { new() { Output = "test_value", Successful = true, ToolName = "test_value" } });
        }

        private async Task PublishReportProfile(FacebookProfile profile, IEnumerable<OutputModel> outputs)
        {
            var reports = outputs
                .Where(model => model.Successful)
                .Select(o => new ToolOutput(o.ToolName, o.Output))
                .ToImmutableList();

            profile.ToolOutputs = reports;
            await _collectorPublisher.PublishToReport(profile);
        }
    }
}