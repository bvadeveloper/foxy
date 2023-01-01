using System.Collections.Generic;

namespace Platform.Tools.Models
{
    /// <summary>
    /// Tool model
    /// </summary>
    public class ToolModel
    {
        public string Name { get; set; }

        public int Timeout { get; set; }

        public string CommandLine { get; set; }

        public string VersionCommandLine { get; set; }

        public List<string> FrameworkTags { get; set; }

        public List<string> ServiceTags { get; set; }

        public List<string> PortTags { get; set; }

        public List<string> HostTags { get; set; }

        public string MakeCommandLine(string target) => string.Format(CommandLine, target);
    }
}