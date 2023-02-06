using Platform.Contract.Reporter.Abstractions;
using Platform.Primitive;

namespace Platform.Contract.Reporter
{
    public class ReportProfile : IReportProfile
    {
        public SessionContext SessionContext { get; set; }

        public string Value { get; set; }

        public ReportModel[] Reports { get; set; }
    }
}