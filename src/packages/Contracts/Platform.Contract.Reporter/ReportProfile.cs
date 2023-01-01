using Platform.Contract.Reporter.Abstractions;
using Platform.Primitive;

namespace Platform.Contract.Reporter
{
    public class ReportProfile : IReportProfile
    {
        public TraceContext TraceContext { get; set; }

        public string Target { get; set; }

        public ReportModel[] Reports { get; set; }
    }
}