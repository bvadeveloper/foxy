using Platform.Primitive;

namespace Platform.Contract.Models;

public class Message<T>
{
    public Message(T value, SessionContext sessionContext)
    {
        Value = value;
        SessionContext = sessionContext;
    }

    public SessionContext SessionContext { get; set; }

    public T Value { get; set; }
}