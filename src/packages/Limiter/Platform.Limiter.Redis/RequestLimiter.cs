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

    public async Task<bool> Acquire(string value)
    {
        var hashString = value.MakeHash();
        var permission = _permissionRepository.Find(hashString);

        return await HasLimit(hashString, DefaultTimeFrame, permission.RequestRate);
    }

    private async Task<bool> HasLimit(string hash, int timeFrame, int rateCount) =>
        ((int)await _redisConnection.Database.ScriptEvaluateAsync(SlidingScript,
            new { key = new RedisKey(hash), timeFrame, permitCount = rateCount })) == 1;
}