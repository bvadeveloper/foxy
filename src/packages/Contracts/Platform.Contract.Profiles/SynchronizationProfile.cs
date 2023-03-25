using System;
using MemoryPack;
using Platform.Contract.Profiles.Enums;
using Platform.Contract.Profiles.Extensions;

namespace Platform.Contract.Profiles;

[MemoryPackable]
public partial record SynchronizationProfile(CollectorInfo CollectorInfo, [property: BrotliFormatter] byte[] IpAddress, [property: BrotliFormatter] byte[] PublicKey) : IProfile
{
    public DateTime CreationDateUtc { get; set; } = DateTime.UtcNow;
}

[MemoryPackable]
public partial record CollectorInfo(string Identifier, string Version, ProcessingTypes ProcessingTypes)
{
    public override string ToString() => $"{ProcessingTypes.ToLower()}:{Identifier}";
}