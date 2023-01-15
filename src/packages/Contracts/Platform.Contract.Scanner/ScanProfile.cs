using System.Net;
using Platform.Contract.Abstractions;
using Platform.Primitive;

namespace Platform.Contract.Scanner
{
    public abstract class ScanProfile : ITarget, IToolProfile
    {
        public string Value { get; set; }

        public string[] Tools { get; set; }

        public SessionContext SessionContext { get; set; }
    }
}