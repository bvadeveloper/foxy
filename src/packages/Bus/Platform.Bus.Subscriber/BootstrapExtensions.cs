using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Platform.Primitives;

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

    public static SessionContext AddContext(this SessionContext sessionContext, byte[] bytes)
    {
        var sessionPayload = Encoding.UTF8.GetString(bytes).Split(':');
        sessionContext.TraceId = sessionPayload[0];
        sessionContext.ChatId = sessionPayload[1];

        return sessionContext;
    }
}