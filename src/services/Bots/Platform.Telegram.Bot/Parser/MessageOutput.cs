using System.Collections.Immutable;
using Platform.Contract.Profiles;

namespace Platform.Telegram.Bot.Parser;

public record MessageOutput(bool IsValid, ImmutableList<CoordinatorProfile> Profiles, string? ValidationInfo)
{
}