using System;
using System.Collections.Immutable;
using MemoryPack;
using Platform.Contract.Profiles.Enums;

namespace Platform.Contract.Profiles.Collectors;

[MemoryPackable]
public partial record HostProfile(string Target, ProcessingTypes ProcessingType, IpLocation IpLocations) : IProfile
{
    public DateTime CreationDateUtc { get; set; } = DateTime.UtcNow;

    public ImmutableList<ToolOutput> ToolOutputs { get; set; } = ImmutableList<ToolOutput>.Empty;
}