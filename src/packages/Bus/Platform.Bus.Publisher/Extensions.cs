using System.Threading.Tasks;
using Platform.Contract.Abstractions;
using Platform.Contract.Models;
using Platform.Primitive;

namespace Platform.Bus.Publisher;

public static class Extensions
{
    public static Task<Result<string>> Publish<T>(this IPublisher publisher, T payload, SessionContext context) where T : ITarget
    {
        var message = new Message<T>(payload, context);
        return publisher.Publish(message);
    }
}