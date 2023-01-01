using Platform.Contract.Abstractions;

namespace Platform.Contract.Reporter.Abstractions
{
    public interface IReportProfile : ITargetProfile
    {
        ReportModel[] Reports { get; set; }
    }
}