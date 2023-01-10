using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Platform.Limiter.Redis.Abstractions;
using Platform.Limiter.Redis.Models;

namespace Platform.Limiter.Redis;

internal class PermissionRepository : IPermissionRepository
{
    private readonly IEnumerable<PermissionModel> _limiterModels;
    private readonly PermissionModel _defaultPermission;

    public PermissionRepository(IOptions<List<PermissionModel>> options)
    {
        _limiterModels = options.Value;
        _defaultPermission = GetDefaultPermission();
    }

    private PermissionModel GetDefaultPermission() =>
        _limiterModels.First(m => m.Hash == "default" && m.Type == UserTypes.Default);

    public PermissionModel FindPermission(string hash) =>
        _limiterModels.FirstOrDefault(m =>
            m.Hash == hash && m.Type is UserTypes.Admin or UserTypes.Advanced or UserTypes.Newcomer) ??
        _defaultPermission;
}