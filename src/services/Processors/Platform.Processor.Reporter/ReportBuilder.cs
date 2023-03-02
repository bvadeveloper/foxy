using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform.Contract.Profiles;

namespace Platform.Processor.Reporter
{
    public class ReportBuilder : IReportBuilder
    {
        public Task<(string, byte[])> BuildTextFileReport(string targetName, ImmutableList<ToolOutput> toolOutputs)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"target: {targetName}");
        
            toolOutputs
                .ToList()
                .ForEach(m =>
                {
                    sb.AppendLine();
                    sb.AppendLine($"processing date (utc): {DateTime.UtcNow}");
                    sb.AppendLine($"tool name: {m.ToolName}");
                    sb.AppendLine();
                    sb.AppendLine(m.Output);
                });
        
            var fileName = $"{targetName}_report_{DateTime.UtcNow:yyyy_MM_dd_HH_mm}.txt";
            var report = Encoding.Default.GetBytes(sb.ToString());
        
            return Task.FromResult((fileName, report));
        }
        
        public Task<string> MakeTextReport(string target, IEnumerable<ToolOutput> models)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"target: {target}");
        
            models
                .ToList()
                .ForEach(m =>
                {
                    sb.AppendLine();
                    sb.AppendLine($"processing date (utc): {DateTime.UtcNow}");
                    sb.AppendLine($"tool name: {m.ToolName}");
                    sb.AppendLine();
                    sb.AppendLine(m.Output);
                });
        
            return Task.FromResult(sb.ToString());
        }
    }
}