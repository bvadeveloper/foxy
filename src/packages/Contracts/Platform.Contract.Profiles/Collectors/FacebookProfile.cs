using System;
using System.Collections.Immutable;
using MemoryPack;
using Platform.Contract.Profiles.Enums;

namespace Platform.Contract.Profiles.Collectors;

[MemoryPackable]
public partial record FacebookProfile(string Target, ProcessingTypes ProcessingType) : IProfile
{
    public DateTime CreationDateUtc { get; set; } = DateTime.UtcNow;
    
    public ImmutableList<ToolOutput> ToolOutputs { get; set; } = ImmutableList<ToolOutput>.Empty;
}