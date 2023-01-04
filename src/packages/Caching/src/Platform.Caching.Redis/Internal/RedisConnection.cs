using System;
using System.Linq;
using StackExchange.Redis;

namespace Platform.Caching.Redis.Internal
{
    public class RedisConnection : IRedisConnection
    {
        private readonly string _redisConfiguration;
        private readonly int _database;
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;

        public RedisConnection(string redisConfiguration, int database = 0)
        {
            _redisConfiguration = redisConfiguration;
            _database = database;
            _connectionMultiplexer =
                new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(redisConfiguration));
        }

        public IDatabaseAsync Database => _connectionMultiplexer.Value.GetDatabase(_database);

        public IServer Server => _connectionMultiplexer.Value
            .GetServer(_redisConfiguration.Split(',').FirstOrDefault()?.Trim());
    }
}