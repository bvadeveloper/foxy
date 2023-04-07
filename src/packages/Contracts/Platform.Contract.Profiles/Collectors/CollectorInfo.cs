using MemoryPack;
using Platform.Contract.Profiles.Enums;
using Platform.Contract.Profiles.Extensions;

namespace Platform.Contract.Profiles.Collectors;

[MemoryPackable]
public partial record CollectorInfo(string Identifier, string Version, ProcessingTypes ProcessingTypes)
{
    public override string ToString() => $"{ProcessingTypes.ToLower()}:{Identifier}";
}