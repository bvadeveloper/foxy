using MemoryPack;
using Platform.Primitives;

namespace Platform.Contract.Profiles;

[MemoryPackable]
public partial record Profile(SessionContext SessionContext, string Message)
{
    [MemoryPackAllowSerialize]
    public SessionContext SessionContext { get; set; }

    public string TargetName { get; set; }
}