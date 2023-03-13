using System;
using MemoryPack;

namespace Platform.Contract.Profiles;

[MemoryPackable]
public partial record ReportProfile(string TargetName) : IProfile
{
    public DateTime CreationDateUtc { get; set; } = DateTime.UtcNow;

    public FileReport FileReport { get; set; }
}

[MemoryPackable]
public partial record FileReport(string FileName, byte[] FileBody)
{
    [BrotliFormatter] public byte[] FileBody { get; set; } = FileBody;

    public string FileName { get; set; } = FileName;
}