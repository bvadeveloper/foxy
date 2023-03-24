using System;
using MemoryPack;

namespace Platform.Contract.Profiles;

[MemoryPackable]
public partial record SynchronizationProfile(CollectorInfo CollectorInfo, [property: BrotliFormatter] byte[] IpAddress, [property: BrotliFormatter] byte[] PublicKey) : IProfile
{
    public DateTime CreationDateUtc { get; set; } = DateTime.UtcNow;
}

[MemoryPackable]
public partial record CollectorInfo(string Identifier, string Version, CollectorTypes CollectorTypes)
{
    public override string ToString() => $"{CollectorTypes.ToString().ToLowerInvariant()}:{Identifier}";
}

public enum CollectorTypes
{
    DomainScanner,
    HostScanner,
    EmailChecker,
    FacebookParser,
    InstagramParser
}