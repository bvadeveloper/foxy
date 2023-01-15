using Platform.Primitive;

namespace Platform.Contract.Abstractions
{
    public interface ITarget
    {
        string Value { get; set; }
        
        SessionContext SessionContext { get; set; }
    }
}