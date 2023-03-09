using System.Collections.Immutable;

namespace Platform.Bus;

/// <summary>
/// Exchange holder for subscribers
/// </summary>
public record ExchangeCollection(ImmutableList<Exchange> Exchanges);

public record Exchange(ExchangeTypes ExchangeTypes, ImmutableList<string> RoutingKeys)
{
    public static Exchange Default(ExchangeTypes exchangeTypes) => new(exchangeTypes, ImmutableList.Create<string>().Add("default"));
}