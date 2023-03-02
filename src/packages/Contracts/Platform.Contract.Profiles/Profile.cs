using System;
using System.Collections.Immutable;
using MemoryPack;

namespace Platform.Contract.Profiles;

[MemoryPackable]
public partial record Profile(string TargetName) : IProfile
{
    public DateTime CreationDateUtc { get; set; } = DateTime.UtcNow;

    public FileReport FileReport { get; set; }

    public ImmutableList<ToolOutput> ToolOutputs { get; set; }
}

[MemoryPackable]
public partial record FileReport(string FileName, byte[] FileBody)
{
    [BrotliFormatter] public byte[] FileBody { get; set; } = FileBody;

    public string FileName { get; set; } = FileName;
}

[MemoryPackable]
public partial record ToolOutput(string ToolName, string Output)
{
}