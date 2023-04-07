using MemoryPack;

namespace Platform.Contract.Profiles.Collectors;

[MemoryPackable]
public partial record IpLocation(string Location, string IpAddress)
{
}