using Platform.Contract.Messages;

namespace Platform.Contract.Reporter.Abstractions
{
    public interface IReportProfile : ITarget
    {
        ReportModel[] Reports { get; set; }
    }
}