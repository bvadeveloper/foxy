using System;
using System.Collections.Generic;

namespace Platform.Contract.Profiles.Extensions;

public static class ProfileExtensions
{
    public static string ToLower(this Enum @enum) => @enum.ToString().ToLowerInvariant();
    
    public static (string item1, string item2) SplitValue(this string value)
    {
        var valueSpan = value.AsSpan();
        var delimiterIndex = valueSpan.IndexOf(':');
        var item1 = valueSpan[..delimiterIndex].ToString();
        var item2 = valueSpan[(delimiterIndex + 1)..].ToString();

        return (item1, item2);
    }

    public static T Random<T>(this IList<T> elements) => elements[new Random().Next(elements.Count)];

    public static string MakeIdentifier() => Guid.NewGuid().ToString("N");
}