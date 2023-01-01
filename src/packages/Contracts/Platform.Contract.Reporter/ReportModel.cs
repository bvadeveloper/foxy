using System;

namespace Platform.Contract.Reporter
{
    public class ReportModel
    {
        public string Output { get; set; }

        public string ToolName { get; set; }

        public DateTime  ProcessingDate { get; set; }
    }
}