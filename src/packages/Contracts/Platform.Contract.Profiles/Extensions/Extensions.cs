using System;

namespace Platform.Contract.Profiles.Extensions;

public static class Extensions
{
    public static string ToLower(this Enum @enum) => @enum.ToString().ToLowerInvariant();
}