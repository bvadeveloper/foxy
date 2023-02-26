using System.Collections.Immutable;
using Platform.Contract.Enums;

namespace Platform.Contract.Models;

/// <summary>
/// Exchange holder for subscribers
/// </summary>
public record Exchanges(ImmutableList<(ExchangeTypes exchangeTypes, string route)> Values);