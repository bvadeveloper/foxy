using System;
using MemoryPack;

namespace Platform.Contract.Profiles;

[MemoryPackable]
public partial record CoordinatorProfile(string[] TargetNames, ProcessingTypes ProcessingTypes, string Options) : IProfile
{
    public DateTime CreationDateUtc { get; set; } = DateTime.UtcNow;

    public static CoordinatorProfile Make(string[] targetNames, ProcessingTypes processingTypes, string options) => new(targetNames, processingTypes, options);
}