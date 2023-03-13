using MemoryPack;

namespace Platform.Contract.Profiles;

[MemoryPackable]
public partial record IpLocation(string Location, string IpAddress)
{
}