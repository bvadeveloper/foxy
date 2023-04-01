using System;
using MemoryPack;

namespace Platform.Contract.Profiles;

[MemoryPackable]
[MemoryPackUnion(0, typeof(CoordinatorProfile))]
[MemoryPackUnion(1, typeof(SynchronizationProfile))]
[MemoryPackUnion(2, typeof(DomainProfile))]
[MemoryPackUnion(3, typeof(HostProfile))]
[MemoryPackUnion(4, typeof(EmailProfile))]
[MemoryPackUnion(5, typeof(FacebookProfile))]
[MemoryPackUnion(6, typeof(ReportProfile))]
public partial interface IProfile
{
    DateTime CreationDateUtc { get; set; }
}


