using Platform.Contract.Abstractions;
using Platform.Primitive;

namespace Platform.Contract
{
    public class IpTarget : ITarget, IToolProfile
    {
        public string Target { get; set; }

        public string[] Tools { get; set; }

        public SessionContext SessionContext { get; set; }
    }
}