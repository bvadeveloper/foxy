using System.Collections.Immutable;

namespace Platform.Bus;

/// <summary>
/// Exchange holder for subscribers
/// </summary>
public record ExchangeCollection(ImmutableList<Exchange> Exchanges);

public record Exchange(ExchangeTypes ExchangeTypes, string RoutingKey = "default");