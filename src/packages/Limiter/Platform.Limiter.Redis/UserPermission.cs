namespace Platform.Limiter.Redis;

public record UserPermission(string Hash, string Nickname, string Description, PermissionTypes Type, int RequestRate)
{
}