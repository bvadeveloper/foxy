using System.Collections.Immutable;

namespace Platform.Bus;

/// <summary>
/// Exchange holder for subscribers
/// </summary>
public record ExchangeCollection(ImmutableList<Exchange> Exchanges);

public record Exchange(string ExchangeName, string RoutingKey = RoutNames.DefaultRoute)
{
    public static Exchange Default(string name) => new(name);
    public static Exchange Make(string name, string route) => new(name, route);
}