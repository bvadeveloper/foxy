using Platform.Contract.Reporter;
using Platform.Contract.Reporter.Abstractions;

namespace Platform.Contract.Telegram
{
    public class TelegramProfile : IReportProfile
    {
        public string Name { get; set; }

        public ReportModel[] Reports { get; set; }
        
        public byte[] FileBody { get; set; }

        public string FileName { get; set; }
    }
}