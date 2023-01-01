using Platform.Contract.Abstractions;
using Platform.Primitive;

namespace Platform.Contract.Scanner
{
    public abstract class ScanProfile : ITargetProfile, IToolProfile
    {
        public string Target { get; set; }

        public string[] Tools { get; set; }

        public TraceContext TraceContext { get; set; }
    }
}