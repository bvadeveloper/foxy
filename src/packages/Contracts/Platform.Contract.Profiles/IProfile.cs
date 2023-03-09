using System;
using MemoryPack;

namespace Platform.Contract.Profiles;

[MemoryPackable]
[MemoryPackUnion(0, typeof(Profile))]
[MemoryPackUnion(1, typeof(SynchronizationProfile))]
public partial interface IProfile
{
    DateTime CreationDateUtc { get; set; }
}