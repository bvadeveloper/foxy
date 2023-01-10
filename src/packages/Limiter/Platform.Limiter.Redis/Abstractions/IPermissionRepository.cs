using Platform.Limiter.Redis.Models;

namespace Platform.Limiter.Redis.Abstractions;

public interface IPermissionRepository
{
    PermissionModel FindPermission(string hash);
}