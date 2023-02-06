using System.Collections.Generic;
using System.Threading.Tasks;
using Platform.Contract;
using Platform.Contract.Reporter;

namespace Platform.Consumer.Reporter.Abstractions
{
    public interface IReportService
    {
        Task<(string, byte[])> MakeFileReport(string target, IEnumerable<ReportModel> reports);
    }
}