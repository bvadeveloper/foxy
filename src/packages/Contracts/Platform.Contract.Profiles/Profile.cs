using MemoryPack;

namespace Platform.Contract.Profiles;

[MemoryPackable]
public partial record Profile(string TargetName) : IProfile
{
}