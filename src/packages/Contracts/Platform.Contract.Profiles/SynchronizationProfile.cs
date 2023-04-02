using System;
using MemoryPack;

namespace Platform.Contract.Profiles;

[MemoryPackable]
public partial record SynchronizationProfile(CollectorInfo CollectorInfo, [property: BrotliFormatter] byte[] IpAddress, [property: BrotliFormatter] byte[] PublicKey) : IProfile
{
    public DateTime CreationDateUtc { get; set; } = DateTime.UtcNow;
}