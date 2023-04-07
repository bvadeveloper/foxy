using System;
using MemoryPack;
using Platform.Contract.Profiles.Enums;

namespace Platform.Contract.Profiles.Processors;

[MemoryPackable]
public partial record CoordinatorProfile(string Target, ProcessingTypes ProcessingType, string Options) : IProfile
{
    public DateTime CreationDateUtc { get; set; } = DateTime.UtcNow;

    public static CoordinatorProfile Make(string targetNames, ProcessingTypes processingTypes, string options) => new(targetNames, processingTypes, options);
    public static CoordinatorProfile Default(string targetNames, ProcessingTypes processingTypes) => new(targetNames, processingTypes, string.Empty);
}