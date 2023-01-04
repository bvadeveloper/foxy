using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Platform.Caching.Redis.Internal;
using Platform.Limiter.Redis.Abstractions;
using Platform.Logging.Extensions;
using StackExchange.Redis;

namespace Platform.Limiter.Redis;

public class RedisRequestLimiter : IRequestLimiter
{
    private const int DefaultTimeFrame = 1;
    
    private readonly IRedisConnection _redisConnection;
    private readonly IEnumerable<LimiterModel> _limiterModels;
    private readonly ILogger _logger;

    public RedisRequestLimiter(
        IRedisConnection redisConnection,
        IOptions<List<LimiterModel>> options,
        ILogger<RedisRequestLimiter> logger)
    {
        _redisConnection = redisConnection;
        _limiterModels = options.Value;
        _logger = logger;
    }

    public async Task<bool> Acquire(string key, int timeFrame, int permitCount)
    {
        var hash = key.MakeHash();

        if (_limiterModels.Any(m => m.Hash == hash && m.Type == UserType.Admin))
        {
            return true;
        }

        if (_limiterModels.Any(m => m.Hash == hash && m.Type == UserType.Advanced))
        {
            return await HasLimitAcquire(hash, DefaultTimeFrame, 10);
        }

        // count of requests for a new user is 1 rpm
        return await HasLimitAcquire(hash, DefaultTimeFrame, 1);
    }

    private async Task<bool> HasLimitAcquire(string input, int timeFrame, int permitCount)
    {
        try
        {
            return ((int)await _redisConnection.Database.ScriptEvaluateAsync(LimiterExtensions.SlidingScript,
                new { key = new RedisKey(input), timeFrame, permitCount })) == 1;
        }
        catch (Exception ex)
        {
            _logger.Error($"Error appeared when calling Redis, '{ex.Message}'", ex);
            return true; // todo: process exception
        }
    }
}