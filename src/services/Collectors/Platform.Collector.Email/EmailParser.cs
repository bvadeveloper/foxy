using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Platform.Contract.Profiles.Collectors;
using Platform.Cryptography;
using Platform.Services.Collector;
using Platform.Tools.Abstractions;
using Platform.Tools.Models;

namespace Platform.Collector.Email
{
    public class EmailParser : IConsumeAsync<EmailProfile>
    {
        private readonly IProcessorClient _collectorClient;
        private readonly IToolsHolder _toolsHolder;
        private readonly ILogger _logger;

        public EmailParser(IToolsHolder toolsHolder, IProcessorClient collectorClient, ILogger<EmailParser> logger)
        {
            _collectorClient = collectorClient;
            _toolsHolder = toolsHolder;
            _logger = logger;
        }

        public async Task ConsumeAsync(EmailProfile profile)
        {
            // var outputs = await _toolsHolder
            //     .FilterByTargetMarks(profile.Tags)
            //     .RunTools(profile.Name);

            await PublishReportProfile(profile, new OutputModel[] { new() { Output = "test_value", Successful = true, ToolName = "test_value" } });
        }

        private async Task PublishReportProfile(EmailProfile profile, IEnumerable<OutputModel> outputs)
        {
            var reports = outputs
                .Where(model => model.Successful)
                .Select(o => new ToolOutput(o.ToolName, o.Output))
                .ToImmutableList();

            profile.ToolOutputs = reports;
            await _collectorClient.SendToReporter(profile);
        }
    }
}