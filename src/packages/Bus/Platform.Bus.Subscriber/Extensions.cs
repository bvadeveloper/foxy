using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Platform.Primitives;
using RabbitMQ.Client.Events;

namespace Platform.Bus.Subscriber;

public static class BootstrapExtensions
{
    /// <summary>
    /// For default routing keys
    /// </summary>
    /// <param name="services"></param>
    /// <param name="exchangeTypes"></param>
    /// <returns></returns>
    public static IServiceCollection AddExchangeListeners(this IServiceCollection services, params ExchangeTypes[] exchangeTypes) =>
        services.AddExchangeListeners(exchangeTypes.Select(Exchange.Default).ToArray());

    public static IServiceCollection AddExchangeListeners(this IServiceCollection services, params Exchange[] exchanges) =>
        services.AddSingleton(new ExchangeCollection(ImmutableList.CreateRange(exchanges)));
}

public static class Extensions
{
    public static void AddContext(this SessionContext sessionContext, byte[] bytes)
    {
        var sessionPayload = Encoding.UTF8.GetString(bytes).Split(':');
        sessionContext.TraceId = sessionPayload[0];
        sessionContext.ChatId = sessionPayload[1];
    }

    public static bool TryGetHeader<T>(this BasicDeliverEventArgs eventArgs, string key, out T? value)
    {
        var hasValue = eventArgs.BasicProperties.Headers.TryGetValue(key, out var @object);
        value = (T)@object!;

        return hasValue;
    }
}