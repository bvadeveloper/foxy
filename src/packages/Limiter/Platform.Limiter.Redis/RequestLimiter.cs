using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Platform.Caching.Redis.Internal;
using Platform.Limiter.Redis.Abstractions;
using Platform.Limiter.Redis.Extensions;
using Platform.Limiter.Redis.Models;
using StackExchange.Redis;

namespace Platform.Limiter.Redis;

public class RequestLimiter : IRequestLimiter
{
    /// <summary>
    /// Default time frame 1 min
    /// </summary>
    private const int DefaultTimeFrame = 1;

    private readonly IRedisConnection _redisConnection;
    private readonly IEnumerable<LimiterModel> _limiterModels;

    public RequestLimiter(IRedisConnection redisConnection, IOptions<List<LimiterModel>> options)
    {
        _redisConnection = redisConnection;
        _limiterModels = options.Value;
    }

    public async Task<bool> Acquire(string input)
    {
        var hash = input.MakeHash();

        if (_limiterModels.Any(m => m.Hash == hash && m.Type == UserType.Admin))
        {
            return true;
        }

        if (_limiterModels.Any(m => m.Hash == hash && m.Type == UserType.Advanced))
        {
            return await HasLimit(hash, DefaultTimeFrame, 10);
        }

        // count of requests for a new user is 1 rpm
        return await HasLimit(hash, DefaultTimeFrame, 1);
    }

    private async Task<bool> HasLimit(string input, int timeFrame, int permitCount) =>
        ((int)await _redisConnection.Database.ScriptEvaluateAsync(LimiterExtensions.SlidingScript,
            new { key = new RedisKey(input), timeFrame, permitCount })) == 1;
}