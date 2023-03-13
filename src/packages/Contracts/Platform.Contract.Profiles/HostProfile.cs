using System;
using System.Collections.Immutable;
using MemoryPack;

namespace Platform.Contract.Profiles;

[MemoryPackable]
public partial record HostProfile(string TargetName, IpLocation IpLocations) : IProfile
{
    public DateTime CreationDateUtc { get; set; } = DateTime.UtcNow;

    public ImmutableList<ToolOutput> ToolOutputs { get; set; } = ImmutableList<ToolOutput>.Empty;
}