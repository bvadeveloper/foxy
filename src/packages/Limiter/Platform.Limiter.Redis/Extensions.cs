using System;
using System.Security.Cryptography;
using System.Text;
using StackExchange.Redis;

namespace Platform.Limiter.Redis
{
    public static class Extensions
    {
        /// <summary>
        /// Make Lua script for sliding windows
        /// </summary>
        public static LuaScript SlidingScript => LuaScript.Prepare(SlidingRateLimiterScript);

        /// <summary>
        /// Sliding rate limiter script
        /// </summary>
        private const string SlidingRateLimiterScript = @"
            local current_time = redis.call('TIME')
            local trim_time = tonumber(current_time[1]) - @timeFrame
            redis.call('ZREMRANGEBYSCORE', @key, 0, trim_time)
            local request_count = redis.call('ZCARD',@key)

            if request_count < tonumber(@permitCount) then
                redis.call('ZADD', @key, current_time[1], current_time[1] .. current_time[2])
                redis.call('EXPIRE', @key, @timeFrame)
                return 0
            end
            return 1
            ";

        /// <summary>
        /// Make hash by string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string MakeHash(this string value)
        {
            using var sha512 = SHA512.Create();
            var bytes = Encoding.Default.GetBytes(value);
            var hashBytes = sha512.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes, Base64FormattingOptions.None);
        }
    }
}