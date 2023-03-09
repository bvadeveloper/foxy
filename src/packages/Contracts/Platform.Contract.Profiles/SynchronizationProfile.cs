using System;
using MemoryPack;

namespace Platform.Contract.Profiles;

[MemoryPackable]
public partial record SynchronizationProfile(string Route, string HostId) : IProfile
{
    public DateTime CreationDateUtc { get; set; } = DateTime.UtcNow;
}