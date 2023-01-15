using Platform.Contract.Abstractions;
using Platform.Primitive;

namespace Platform.Contract
{
    public record IpTarget : ITarget, IToolProfile
    {
        public string Value { get; set; }

        public string[] Tools { get; set; }

        public SessionContext SessionContext { get; set; }
    }
}