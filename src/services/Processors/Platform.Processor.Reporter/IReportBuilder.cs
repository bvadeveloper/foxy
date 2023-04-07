using System.Collections.Immutable;
using System.Threading.Tasks;
using Platform.Contract.Profiles;

namespace Platform.Processor.Reporter
{
    public interface IReportBuilder
    {
        Task<(string, byte[])> BuildTextFileReport(string target, ImmutableList<ToolOutput> toolOutputs);
    }
}