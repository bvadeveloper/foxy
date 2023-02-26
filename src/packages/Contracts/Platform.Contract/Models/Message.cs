using Platform.Contract.Abstractions;
using Platform.Primitive;

namespace Platform.Contract.Models;

/// <summary>
/// Request wrapper for AMQP communication
/// </summary>
/// <param name="Payload"></param>
/// <param name="Context"></param>
/// <typeparam name="T"></typeparam>
public record Message<T>(T Payload, SessionContext Context) where T : ITarget
{
    public SessionContext Context { get; set; } = Context;

    public T Payload { get; set; } = Payload;
}