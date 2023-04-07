using System;
using MemoryPack;

namespace Platform.Contract.Profiles;

[MemoryPackable]
[MemoryPackUnion(0, typeof(Processors.CoordinatorProfile))]
[MemoryPackUnion(1, typeof(Processors.SynchronizationProfile))]
[MemoryPackUnion(2, typeof(Processors.ReportProfile))]
[MemoryPackUnion(3, typeof(Collectors.DomainProfile))]
[MemoryPackUnion(4, typeof(Collectors.HostProfile))]
[MemoryPackUnion(5, typeof(Collectors.EmailProfile))]
[MemoryPackUnion(6, typeof(Collectors.FacebookProfile))]
public partial interface IProfile
{
    DateTime CreationDateUtc { get; set; }
}


