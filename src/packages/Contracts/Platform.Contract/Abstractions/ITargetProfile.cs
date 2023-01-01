using Platform.Primitive;

namespace Platform.Contract.Abstractions
{
    public interface ITargetProfile
    {
        string Target { get; set; }
        
        TraceContext TraceContext { get; set; }
    }
}