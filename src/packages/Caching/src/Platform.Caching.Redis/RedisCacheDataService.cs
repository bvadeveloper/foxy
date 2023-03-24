using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Platform.Caching.Abstractions;
using Platform.Caching.Redis.Internal;
using StackExchange.Redis;
using Convert = Platform.Caching.Redis.Internal.Convert;

namespace Platform.Caching.Redis
{
    using static Convert;

    public class RedisCacheDataService : ICacheDataService
    {
        private const int MaxValuesChunkSize = 32;
        private const int ScanKeysPageSize = 10000;

        private readonly Lazy<IRedisConnection> _connection;
        private IDatabaseAsync Database => _connection.Value.Database;
        private IServer Server => _connection.Value.Server;

        public RedisCacheDataService(Lazy<IRedisConnection> connection) => _connection = connection;


        #region Key/Value Operations

        public async Task<T> GetValue<T>(string key)
        {
            var currentValue = await Database.StringGetAsync(key);
            return string.IsNullOrWhiteSpace(currentValue)
                ? default
                : Parse<T>(currentValue);
        }

        public async Task<string> GetValue(string key)
        {
            var currentValue = await Database.StringGetAsync(key);
            return string.IsNullOrWhiteSpace(currentValue)
                ? default
                : currentValue.ToString();
        }

        public async Task<string[]> GetValues(string key, params string[] keys)
        {
            var redisKeys = new RedisKey[] { key }.Union(keys.Select(k => (RedisKey)k)).ToArray();
            var results = await Database.StringGetAsync(redisKeys);
            return results.Select(r => (string)r ?? string.Empty).ToArray();
        }

        public async Task<bool> KeyExists(string key) =>
            await Database.KeyExistsAsync(key);

        public async Task<long> Increment(string key) =>
            await Database.StringIncrementAsync(key, 1L, Wait(true));

        public async Task SetExpiry(string key, TimeSpan expiry, bool wait) =>
            await Database.KeyExpireAsync(key, expiry, Wait(wait));

        public async Task SetValue(string key, string value, TimeSpan? expiry, bool wait) =>
            await Database.StringSetAsync(key, value, expiry, When.Always, Wait(wait));

        public async Task SetValue<T>(string key, T value, TimeSpan? expiry, bool wait)
        {
            var stringValue = Serialize(value);
            await Database.StringSetAsync(key, stringValue, expiry, When.Always, Wait(wait));
        }

        public async Task<bool> SetValueIfNotExists<T>(string key, T value) =>
            0 != (int)await Database.ExecuteAsync($"SETNX", key, Serialize(value));

        public async Task Delete(string key, bool wait) =>
            await Database.KeyDeleteAsync(key, Wait(wait));

        public async Task<long> Delete(string[] keys, bool wait) =>
            await Database.KeyDeleteAsync(keys.Select(s => (RedisKey)s).ToArray(), Wait(wait));

        public async Task DeleteByPattern(string pattern)
        {
            var keys = Server.Keys(0, pattern, ScanKeysPageSize).ToArray();
            foreach (var redisKey in keys)
            {
                await Database.KeyDeleteAsync(redisKey, CommandFlags.FireAndForget);
            }
        }

        #endregion

        #region Set Operations

        public async Task AppendSetValue(string key, string value, bool wait) =>
            await Database.SetAddAsync(key, value, Wait(wait));

        public async Task AppendSetValue<T>(string key, T value, bool wait) =>
            await Database.SetAddAsync(key, Serialize(value), Wait(wait));

        public async Task AppendSetValues<T>(string key, IEnumerable<T> values, bool wait)
            where T : class
        {
            var items = values
                .Select((x, i) => new { x, i })
                .GroupBy(tup => tup.i / MaxValuesChunkSize)
                .Select(gs => gs.Select(x => x.x).ToList())
                .ToList();

            var cf = Wait(wait);

            foreach (var xs in items)
            {
                var index = 0;
                var redisValues = new RedisValue[xs.Count];
                foreach (var value in xs)
                {
                    redisValues[index] = Serialize(value);
                    index += 1;
                }

                await Database.SetAddAsync(key, redisValues, cf);
            }
        }

        public async Task<IReadOnlyList<string>> GetSetValues(string key)
        {
            var values = await Database.SetMembersAsync(key);
            return values == null ? new List<string>() : values.Select(x => x.ToString()).ToList();
        }

        public async Task<IReadOnlyList<T>> GetSetValues<T>(string key)
        {
            var values = await Database.SetMembersAsync(key);
            return values == null ? new List<T>() : values.Select(x => Parse<T>(x)).Where(x => x != null).ToList();
        }

        #endregion

        #region Hash Operations

        public async Task<bool> HashExists(string key, string hashKey) =>
            await Database.HashExistsAsync(key, hashKey);

        public async Task<string> GetHashValue(string key, string hashKey)
        {
            var result = (string)await Database.HashGetAsync(key, hashKey);
            return string.IsNullOrEmpty(result) ? string.Empty : result;
        }

        public async Task<T> GetHashValue<T>(string key, string hashKey)
        {
            var stringValue = await Database.HashGetAsync(key, hashKey);
            return Parse<T>(stringValue);
        }

        public async Task<Dictionary<string, T>> GetAllHashValues<T>(string key)
        {
            var hash = await Database.HashGetAllAsync(key);
            var entries = hash?.ToDictionary(e => e.Name.ToString(), e => Parse<T>(e.Value));

            return entries;
        }

        public async Task DeleteHashValues(string key, IEnumerable<string> hashKeys, bool wait) =>
            await Database.HashDeleteAsync(key, hashKeys.Select(k => (RedisValue)k).ToArray(), Wait(wait));

        public async Task<bool> SetHashValue(string key, string hashKey, string value, bool wait) =>
            await Database.HashSetAsync(key, hashKey, value, When.Always, Wait(wait));

        public async Task<bool> SetHashValue<T>(string key, string hashKey, T value, bool wait)
        {
            var stringValue = Serialize(value);
            return await Database.HashSetAsync(key, hashKey, stringValue, When.Always, Wait(wait));
        }

        private static CommandFlags Wait(bool wait) =>
            wait
                ? CommandFlags.None
                : CommandFlags.FireAndForget;

        #endregion

        #region Scan Operations

        /// <summary>
        /// Pattern ("value*", 1)
        /// </summary>
        /// <param name="match">"value*"</param>
        /// <param name="count">int</param>
        /// <returns></returns>
        public async Task<List<string>> KeyScan(string match, int count)
        {
            var schemas = new List<string>();
            var nextCursor = 0;
            do
            {
                var redisResult = await Database.ExecuteAsync("SCAN", nextCursor.ToString(), "MATCH", match, "COUNT", count.ToString());
                var innerResult = (RedisResult[])redisResult;

                nextCursor = int.Parse((string)innerResult[0]);

                var resultLines = ((string[])innerResult[1]).ToList();
                schemas.AddRange(resultLines);
            } while (nextCursor != 0);

            return schemas;
        }

        #endregion
    }
}