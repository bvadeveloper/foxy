using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Platform.Contract.Enums;
using Platform.Tools.Extensions;
using Microsoft.Extensions.Options;
using Platform.Tools.Abstractions;
using Platform.Tools.Models;

namespace Platform.Tools
{
    public class ToolsHolder : IToolsHolder
    {
        /// <summary>
        /// Set of tools from environment variables
        /// </summary>
        private readonly IEnumerable<ToolModel> _tools;

        public ToolsHolder(IOptions<List<ToolModel>> options) => _tools = options.Value;

        public async Task<OutputModel[]> RunTools(string target) =>
            await Task.WhenAll(_tools.RunToolProcesses(target));

        public async Task<OutputModel[]> RunVersion() =>
            await Task.WhenAll(_tools.RunToolsVersionProcesses());

        public IToolsHolder FilterByTools(IEnumerable<string> optionalTools) =>
            new ToolsHolder(new OptionsWrapper<List<ToolModel>>(_tools.Where(m => optionalTools.Contains(m.Name)).ToList()));

        public IToolsHolder FilterByTargetMarks(Dictionary<TargetType, List<string>> marks)
        {
            if (marks.Any())
            {
                static Func<string, bool> TargetContains(ICollection<string> targetTags) => toolTag => targetTags?.Contains(toolTag) ?? false;

                var mappedTools = marks
                    .SelectMany(pair =>
                        _tools
                            .Where(toolTag =>
                                pair.Value.Any(TargetContains(toolTag.FrameworkTags))
                                || pair.Value.Any(TargetContains(toolTag.HostTags))
                                || pair.Value.Any(TargetContains(toolTag.PortTags))
                                || pair.Value.Any(TargetContains(toolTag.ServiceTags)))
                            .Select(tool => tool.Name)
                    ).Distinct();

                return FilterByTools(mappedTools);
            }

            return this;
        }
    }
}