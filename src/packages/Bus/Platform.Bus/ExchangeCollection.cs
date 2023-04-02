using System.Collections.Immutable;
using Platform.Bus.Constants;

namespace Platform.Bus;

/// <summary>
/// Exchange holder for subscribers
/// </summary>
public record ExchangeCollection(ImmutableList<Exchange> Exchanges);

public record Exchange(ExchangeTypes ExchangeTypes, string RoutingKey = BusConstants.DefaultRoute)
{
    public static Exchange Default(ExchangeTypes type) => new(type);
    public static Exchange Make(ExchangeTypes type, string route) => new(type, route);
}