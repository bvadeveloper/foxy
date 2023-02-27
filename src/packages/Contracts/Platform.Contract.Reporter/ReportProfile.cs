using Platform.Contract.Reporter.Abstractions;

namespace Platform.Contract.Reporter
{
    public class ReportProfile : IReportProfile
    {
        public string Name { get; set; }

        public ReportModel[] Reports { get; set; }
    }
}