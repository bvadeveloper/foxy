using System.Text;
using Platform.Contract.Profiles.Extensions;
using Platform.Cryptography;
using Platform.Primitives;
using RabbitMQ.Client.Events;

namespace Platform.Bus.Subscriber;

internal static class Extensions
{
    internal static void AddContext(this SessionContext sessionContext, byte[] bytes)
    {
        var (traceId, chatId) = Encoding.UTF8.GetString(bytes).SplitValue();
        sessionContext.CorrelationId = traceId;
        sessionContext.SessionId = chatId;
    }

    internal static void AddPublicKey(this PublicKeyHolder keyHolder, byte[] publicKey) => keyHolder.Value = publicKey;

    internal static bool TryGetHeader<T>(this BasicDeliverEventArgs eventArgs, string key, out T? value)
    {
        var hasValue = eventArgs.BasicProperties.Headers.TryGetValue(key, out var @object);
        value = (T)@object!;

        return hasValue;
    }
}