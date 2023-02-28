using System.Collections.Immutable;

namespace Platform.Bus;

/// <summary>
/// Exchange holder for subscribers
/// </summary>
public record ExchangeCollection
{
    /// <summary>
    /// Exchange holder for subscribers
    /// </summary>
    public ExchangeCollection(ImmutableList<Exchange> Exchanges)
    {
        this.Exchanges = Exchanges;
    }

    public ImmutableList<Exchange> Exchanges { get; init; }

    public void Deconstruct(out ImmutableList<Exchange> Exchanges)
    {
        Exchanges = this.Exchanges;
    }
}

public record Exchange(ExchangeTypes ExchangeTypes, string RoutingKey = "default");
