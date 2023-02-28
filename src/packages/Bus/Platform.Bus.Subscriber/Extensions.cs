using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Bus.Subscriber;

public static class Extensions
{
    public static IServiceCollection AddExchanges(this IServiceCollection services, ExchangeCollection exchanges) =>
        services.AddSingleton(exchanges);

    public static IServiceCollection AddExchange(this IServiceCollection services, ExchangeTypes exchangeTypes, string routingKey = "default") =>
        services.AddSingleton(new ExchangeCollection(ImmutableList.Create(new Exchange(exchangeTypes, routingKey))));
}