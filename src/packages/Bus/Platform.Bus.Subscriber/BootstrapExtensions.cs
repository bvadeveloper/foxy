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
    /// <param name="exchangeNames"></param>
    /// <param name="routeKey"></param>
    /// <returns></returns>
    public static IServiceCollection AddExchanges(this IServiceCollection services, string[] exchangeNames, string routeKey = RoutNames.DefaultRoute) =>
        services.AddSingleton(_ =>
        {
            var exchanges = exchangeNames
                .Select(exchangeName => Exchange.Make(exchangeName, routeKey))
                .ToImmutableList();

            return new ExchangeCollection(exchanges);
        });
}