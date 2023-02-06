using System.Collections.Immutable;
using Platform.Contract.Enums;

namespace Platform.Contract.Models;

/// <summary>
/// Exchange holder for subscribers
/// </summary>
public class Exchanges
{
    public Exchanges(ImmutableList<(ExchangeTypes Exchange, string Route)> values) => Values = values;

    public ImmutableList<(ExchangeTypes Exchange, string Route)> Values { get; }
}