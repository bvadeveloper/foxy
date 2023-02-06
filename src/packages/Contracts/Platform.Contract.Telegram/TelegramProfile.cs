using Platform.Contract.Reporter;
using Platform.Contract.Reporter.Abstractions;
using Platform.Primitive;

namespace Platform.Contract.Telegram
{
    public class TelegramProfile : IReportProfile
    {
        public SessionContext SessionContext { get; set; }

        public string Value { get; set; }

        public ReportModel[] Reports { get; set; }
        
        public byte[] FileBody { get; set; }

        public string FileName { get; set; }
    }
}