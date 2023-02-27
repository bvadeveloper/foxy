using System.Collections.Immutable;

namespace Platform.Bus;

/// <summary>
/// Exchange holder for subscribers
/// </summary>
public record RoutesHolder(ImmutableList<(RouteTypes exchangeTypes, string route)> Values);