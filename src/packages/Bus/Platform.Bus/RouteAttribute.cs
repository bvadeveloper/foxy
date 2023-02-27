using System;

namespace Platform.Bus;

[AttributeUsage(AttributeTargets.Interface)]
public class RouteAttribute : Attribute
{
    public RouteTypes Exchange { get; }
    public string Route { get; }

    public RouteAttribute(RouteTypes exchange, string route = "default")
    {
        Exchange = exchange;
        Route = route;
    }
}