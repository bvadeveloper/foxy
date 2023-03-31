using System.Collections.Immutable;

namespace Platform.Bus;

/// <summary>
/// Exchange holder for subscribers
/// </summary>
public record ExchangeCollection(ImmutableList<Exchange> Exchanges);

public record Exchange(ExchangeTypes ExchangeTypes, string RoutingKey)
{
    public static Exchange Default(ExchangeTypes type) => new(type, "default");
    public static Exchange Make(ExchangeTypes type, string route) => new(type, route);
}