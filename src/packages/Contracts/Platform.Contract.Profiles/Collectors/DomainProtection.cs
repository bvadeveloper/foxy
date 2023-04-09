using MemoryPack;
using Platform.Contract.Profiles.Enums;

namespace Platform.Contract.Profiles.Collectors;

[MemoryPackable]
public partial record DomainProtection(DomainProtectionTypes ProtectionType)
{
}