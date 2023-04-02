using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Bus.Subscriber;

public static class BootstrapExtensions
{
    /// <summary>
    /// Add exchanges and routing key
    /// </summary>
    /// <param name="services"></param>
    /// <param name="exchangeTypes"></param>
    /// <param name="routeKey"></param>
    /// <returns></returns>
    public static IServiceCollection AddExchanges(this IServiceCollection services, ExchangeTypes[] exchangeTypes, string routeKey = BusConstants.DefaultRoute) =>
        services.AddSingleton(_ =>
        {
            var exchanges = exchangeTypes
                .Select(exchangeType => Exchange.Make(exchangeType, routeKey))
                .ToImmutableList();

            return new ExchangeCollection(exchanges);
        });
}