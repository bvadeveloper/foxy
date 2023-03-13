using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Profiles;
using Platform.Tools.Abstractions;
using Platform.Tools.Models;

namespace Platform.Scanners.Facebook
{
    public class FacebookScanner : IConsumeAsync<FacebookProfile>
    {
        private readonly IToolsHolder _toolsHolder;
        private readonly IBusPublisher _publishClient;
        private readonly ILogger _logger;

        public FacebookScanner(IToolsHolder toolsHolder, IBusPublisher publishClient, ILogger<FacebookScanner> logger)
        {
            _toolsHolder = toolsHolder;
            _publishClient = publishClient;
            _logger = logger;
        }

        public async ValueTask ConsumeAsync(FacebookProfile profile)
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
            await _publishClient.PublishToReportExchange(profile);
        }
    }
}