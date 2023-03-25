using System;
using MemoryPack;
using Platform.Contract.Profiles.Enums;

namespace Platform.Contract.Profiles;

[MemoryPackable]
public partial record SynchronizationProfile(CollectorInfo CollectorInfo, [property: BrotliFormatter] byte[] IpAddress, [property: BrotliFormatter] byte[] PublicKey) : IProfile
{
    public DateTime CreationDateUtc { get; set; } = DateTime.UtcNow;
}

[MemoryPackable]
public partial record CollectorInfo(string Identifier, string Version, CollectorTypes CollectorTypes)
{
    public override string ToString() => $"{CollectorTypes.ToLower()}:{Identifier}";
}