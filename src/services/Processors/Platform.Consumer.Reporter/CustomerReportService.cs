using Platform.Consumer.Reporter.Abstractions;

namespace Platform.Consumer.Reporter
{
    public class CustomerReportService : IReportService
    {
        // public Task<(string, byte[])> MakeFileReport(string target, IEnumerable<ReportModel> models)
        // {
        //     var sb = new StringBuilder();
        //     sb.AppendLine($"target: {target}");
        //
        //     models
        //         .ToList()
        //         .ForEach(m =>
        //         {
        //             sb.AppendLine();
        //             sb.AppendLine($"processing date: {m.ProcessingDate}");
        //             sb.AppendLine($"tool name: {m.ToolName}");
        //             sb.AppendLine();
        //             sb.AppendLine(m.Output);
        //         });
        //
        //     var fileName = $"{target}_report_{DateTime.UtcNow:yyMMddHHmm}.txt";
        //     var report = Encoding.Default.GetBytes(sb.ToString());
        //
        //     return Task.FromResult((fileName, report));
        // }
        //
        // public Task<string> MakeTextReport(string target, IEnumerable<ReportModel> models)
        // {
        //     var sb = new StringBuilder();
        //     sb.AppendLine($"target: {target}");
        //
        //     models
        //         .ToList()
        //         .ForEach(m =>
        //         {
        //             sb.AppendLine();
        //             sb.AppendLine($"processing date: {m.ProcessingDate}");
        //             sb.AppendLine($"tool name: {m.ToolName}");
        //             sb.AppendLine();
        //             sb.AppendLine(m.Output);
        //         });
        //
        //     return Task.FromResult(sb.ToString());
        // }
    }
}