using System;

namespace Platform.Tools.Models
{
    public class OutputModel
    {
        public string Output { get; set; }

        public string ErrorOutput { get; set; }

        public string ToolName { get; set; }
        
        public bool Successful { get; set; }
        
        public Exception ExecutionException { get; set; }
    }
}