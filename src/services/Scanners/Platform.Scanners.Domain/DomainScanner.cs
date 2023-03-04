using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Bus;
using Platform.Bus.Publisher;
using Platform.Bus.Subscriber;
using Platform.Contract.Enums;
using Platform.Contract.Profiles;
using Platform.Primitives;
using Platform.Tools;
using Platform.Tools.Abstractions;
using Platform.Tools.Models;

namespace Platform.Scanners.Domain
{
    public class DomainScanner : IConsumeAsync<Profile>
    {
        private readonly IToolsHolder _toolsHolder;
        private readonly IBusPublisher _publisher;
        private readonly ILogger _logger;
        
        public DomainScanner(
            IToolsHolder toolsHolder,
            IBusPublisher publishClient,
            ILogger<DomainScanner> logger)
        {
            _toolsHolder = toolsHolder;
            _publisher = publishClient;
            _logger = logger;
        }
        
        public async ValueTask ConsumeAsync(Profile profile)
        {
            if (true)//(target.Tools.Any())
            {
                // 1. send profile to scanners
                await PublishScanProfile(profile);
            }
            else
            {
                // // 1. start collect tools, fill target tags
                // var (outputs, tags) = await CollectTargetTags(profile.Name);
                //
                // if (tags.ContainsKey(TargetType.NotAvailable))
                // {
                //     // target is NOT available
                //
                //     // 4. send target tags to report host for sending to clients
                //     await PublishReportProfile(profile, outputs);
                // }
                // else
                // {
                //     // target is available
                //
                //     // 2. send scan profile to scanners
                //     await PublishScanProfile(profile, tags);
                //
                //     // 3. send request to save target tags to DB
                //     // await PublishSaveProfile(profile, tags);
                // }
            }
        }
        
        private async Task PublishScanProfile(Profile profile, Dictionary<TargetType, List<string>>? tags = default)
        {
            await _publisher.PublishToHostExchange(profile);
        }
        
        // private async Task PublishReportProfile(ITarget profile, IEnumerable<OutputModel> outputs)
        // {
        //     var reports = outputs
        //         .Where(toolOutput => toolOutput.Successful)
        //         .Select(toolOutput => new ReportModel
        //         {
        //             ToolName = toolOutput.ToolName,
        //             Output = toolOutput.Output,
        //             ProcessingDate = DateTime.UtcNow
        //         })
        //         .ToArray();
        //
        //     await _publisher.Publish(new ReportProfile
        //     {
        //         Value = profile.Value,
        //         Reports = reports
        //     });
        // }
        
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