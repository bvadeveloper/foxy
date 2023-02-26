using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Platform.Contract;
using Platform.Contract.Models;

namespace Platform.Bus.Subscriber;

public static class Extensions
{
    public static IServiceCollection AddExchanges(this IServiceCollection services, params Type[] types)
    {
        var mapping = types.Select(type =>
        {
            var attr = Attribute.GetCustomAttribute(type, typeof(ExchangeAttribute), true) as ExchangeAttribute;
            return (attr.Exchange, attr.Route);
        }).ToImmutableList();

        return services.AddSingleton(new Exchanges(mapping));
    }
}