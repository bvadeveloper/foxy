using System.Threading.Tasks;
using Platform.Caching.Redis.Internal;
using Platform.Limiter.Redis.Abstractions;
using StackExchange.Redis;
using static Platform.Limiter.Redis.Extensions;

namespace Platform.Limiter.Redis;

public class RequestLimiter : IRequestLimiter
{
    /// <summary>
    /// Default time frame 1 min
    /// </summary>
    private const int DefaultTimeFrame = 60;

    private readonly IRedisConnection _redisConnection;
    private readonly IPermissionRepository _permissionRepository;

    public RequestLimiter(IRedisConnection redisConnection, IPermissionRepository permissionRepository)
    {
        _redisConnection = redisConnection;
        _permissionRepository = permissionRepository;
    }

    public async Task<bool> Acquire(string input)
    {
        var hash = input.MakeHash();
        var permissionModel = _permissionRepository.FindPermission(hash);

        return await HasLimit(hash, DefaultTimeFrame, permissionModel.RequestRate);
    }

    private async Task<bool> HasLimit(string input, int timeFrame, int permitCount) =>
        ((int)await _redisConnection.Database.ScriptEvaluateAsync(SlidingScript,
            new { key = new RedisKey(input), timeFrame, permitCount })) == 1;
}