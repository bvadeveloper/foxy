using StackExchange.Redis;

namespace Platform.Caching.Redis.Internal
{
    public interface IRedisConnection
    {
        IDatabaseAsync Database { get; }

        IServer Server { get; }
    }
}
