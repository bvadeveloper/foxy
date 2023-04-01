using System;
using System.Collections.Generic;

namespace Platform.Processor.Coordinator;

internal static class Extensions
{
    internal static (string, string) SplitValue(this string value)
    {
        var valueSpan = value.AsSpan();
        var delimiterIndex = valueSpan.IndexOf(':');
        var item1 = valueSpan[..delimiterIndex].ToString();
        var item2 = valueSpan[(delimiterIndex + 1)..].ToString();

        return (item1, item2);
    }

    internal static T Random<T>(this IList<T> elements) => elements[new Random().Next(elements.Count)];
}