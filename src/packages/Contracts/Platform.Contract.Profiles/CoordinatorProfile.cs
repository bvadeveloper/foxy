using System;
using MemoryPack;

namespace Platform.Contract.Profiles;

[MemoryPackable]
public partial record CoordinatorProfile(string TargetName, ProcessingTypes ProcessingTypes, string Options) : IProfile
{
    public DateTime CreationDateUtc { get; set; } = DateTime.UtcNow;

    public static CoordinatorProfile Make(string targetName, ProcessingTypes processingTypes, string options) => new(targetName, processingTypes, options);
}