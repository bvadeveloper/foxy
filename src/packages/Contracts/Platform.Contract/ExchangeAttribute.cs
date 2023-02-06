using System;
using Platform.Contract.Enums;

namespace Platform.Contract;

[AttributeUsage(AttributeTargets.Interface)]
public class ExchangeAttribute : Attribute
{
    public ExchangeTypes Exchange { get; }
    public string Route { get; }

    public ExchangeAttribute(ExchangeTypes exchange, string route = "default")
    {
        Exchange = exchange;
        Route = route;
    }
}