namespace Platform.Limiter.Redis;

public record UserPermission
{
    public string Hash { get; init; }
    public string Nickname { get; init; }
    public string Description { get; init; }
    public PermissionTypes Type { get; init; }
    public int RequestRate { get; init; }
}