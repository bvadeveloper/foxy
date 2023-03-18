using System.Collections.Immutable;
using Platform.Contract.Profiles;

namespace BotMessageParser;

public record ParseResult(bool IsValid, ImmutableArray<CoordinatorProfile> Profiles, string? ValidationInfo)
{
}