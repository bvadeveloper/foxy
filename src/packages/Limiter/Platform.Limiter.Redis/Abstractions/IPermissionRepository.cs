namespace Platform.Limiter.Redis.Abstractions;

public interface IPermissionRepository
{
    UserPermission Find(string hash);
}