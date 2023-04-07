using System.Collections.Immutable;
using Platform.Contract.Profiles.Processors;

namespace Platform.Telegram.Bot.Parsers;

public record ParseResult(bool IsValid, ImmutableArray<CoordinatorProfile> Profiles, string? ValidationInfo)
{
}