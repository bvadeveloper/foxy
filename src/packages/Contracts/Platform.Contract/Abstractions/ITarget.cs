using Platform.Primitive;

namespace Platform.Contract.Abstractions
{
    public interface ITarget
    {
        string Target { get; set; }
        
        SessionContext SessionContext { get; set; }
    }
}