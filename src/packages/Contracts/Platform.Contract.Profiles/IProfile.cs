using MemoryPack;

namespace Platform.Contract.Profiles;

[MemoryPackable]
[MemoryPackUnion(0, typeof(Profile))]
public partial interface IProfile
{
}