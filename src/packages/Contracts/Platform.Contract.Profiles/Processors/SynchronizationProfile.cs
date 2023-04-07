using System;
using MemoryPack;
using Platform.Contract.Profiles.Collectors;

namespace Platform.Contract.Profiles.Processors;

[MemoryPackable]
public partial record SynchronizationProfile(CollectorInfo CollectorInfo, [property: BrotliFormatter] byte[] IpAddress, [property: BrotliFormatter] byte[] PublicKey) : IProfile
{
    public DateTime CreationDateUtc { get; set; } = DateTime.UtcNow;
}