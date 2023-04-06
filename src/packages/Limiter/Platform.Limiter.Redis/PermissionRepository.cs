using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Platform.Limiter.Redis.Abstractions;

namespace Platform.Limiter.Redis;

internal class PermissionRepository : IPermissionRepository
{
    private const string DefaultPermissionName = "default";

    private readonly IEnumerable<UserPermission> _userPermissions;
    private readonly UserPermission _defaultPermission;

    public PermissionRepository(IOptions<List<UserPermission>> options)
    {
        _userPermissions = options.Value;
        _defaultPermission = GetDefault();
    }

    private UserPermission GetDefault() => _userPermissions.First(m => m is { Hash: DefaultPermissionName, Type: PermissionTypes.Default });

    public UserPermission Find(string value) =>
        _userPermissions.FirstOrDefault(m => m.Hash == value && m.Type.HasFlag(PermissionTypes.NotDefault)) ?? _defaultPermission;
}