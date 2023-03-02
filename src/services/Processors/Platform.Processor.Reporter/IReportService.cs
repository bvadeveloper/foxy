using System.Collections.Immutable;
using System.Threading.Tasks;
using Platform.Contract.Profiles;

namespace Platform.Processor.Reporter
{
    public interface IReportService
    {
        Task<(string, byte[])> MakeFileReport(string target, ImmutableList<ToolOutput> reports);
    }
}