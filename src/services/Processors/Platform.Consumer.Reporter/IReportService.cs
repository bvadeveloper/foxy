using System.Collections.Immutable;
using System.Threading.Tasks;
using Platform.Contract.Profiles;

namespace Platform.Consumer.Reporter
{
    public interface IReportService
    {
        Task<(string, byte[])> MakeFileReport(string target, ImmutableList<ToolOutput> reports);
    }
}