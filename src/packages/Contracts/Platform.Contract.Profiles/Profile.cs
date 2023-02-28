using Platform.Primitives;

namespace Platform.Contract.Profiles;

public record Profile
{
    public SessionContext SessionContext { get; set; }

    public string TargetName { get; set; }
}