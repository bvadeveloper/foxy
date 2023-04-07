using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Platform.Caching.Abstractions
{
    public interface IReadCache
    {
        /// <summary>
        /// Get value by key
        /// </summary>
        Task<T> GetValue<T>(string key);

        /// <summary>
        /// Get string value by key
        /// </summary>
        Task<string> GetValue(string key);

        /// <summary>
        /// Get values by range of keys
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task<string[]> GetValues(string key, params string[] keys);

        /// <summary>
        /// Check if a particular key exists in the cache
        /// </summary>
        Task<bool> KeyExists(string key);
    }

    public interface IWriteCache
    {
        /// <summary>
        /// Increment value
        /// </summary>
        /// <param name="key"></param>
        Task<long> Increment(string key);

        /// <summary>
        /// Sets expiration time to key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <param name="wait"></param>
        Task SetExpiry(string key, TimeSpan expiry, bool wait);

        /// <summary>
        /// Set string value to cache
        /// </summary>
        Task SetValue(string key, string value, TimeSpan? expiry, bool wait);

        /// <summary>
        /// Set object value to cache
        /// </summary>
        Task SetValue<T>(string key, T value, TimeSpan? expiry, bool wait);

        /// <summary>
        /// Set object value to cache if the specified key does not exist.
        /// </summary>
        /// <returns>True if the key was set.</returns>
        Task<bool> SetValueIfNotExists<T>(string key, T value);

        /// <summary>
        /// Delete value in cache by key
        /// </summary>
        Task Delete(string key, bool wait);

        /// <summary>
        /// Delete values in cache by keys
        /// </summary>
        Task<long> Delete(string[] keys, bool wait);

        /// <summary>
        /// Delete value in cache by RegEx expression
        /// </summary>
        Task DeleteByPattern(string pattern);
    }

    public interface IReadSetCache
    {
        /// <summary>
        /// Get set values by key
        /// </summary>
        Task<IReadOnlyList<string>> GetSetValues(string key);

        /// <summary>
        /// Get set values by key
        /// </summary>
        Task<IReadOnlyList<T>> GetSetValues<T>(string key);
    }

    public interface IWriteSetCache
    {
        /// <summary>
        /// Add one member to a set
        /// </summary>
        Task AppendSetValue(string key, string value, bool wait);

        /// <summary>
        /// Add one member to a set
        /// </summary>
        Task AppendSetValue<T>(string key, T value, bool wait);

        /// <summary>
        /// Add one or more members to a set
        /// </summary>
        Task AppendSetValues<T>(string key, IEnumerable<T> values, bool wait) where T : class;
    }

    public interface IReadHashCache
    {
        /// <summary>
        /// Returns whether cache contains hash value
        /// </summary>
        Task<bool> HashExists(string key, string hashKey);

        /// <summary>
        /// Get hash value
        /// </summary>
        Task<string> GetHashValue(string key, string hashKey);

        /// <summary>
        /// Get hash value. Generic version
        /// </summary>
        Task<T> GetHashValue<T>(string key, string hashKey);

        /// <summary>
        /// Get all hash
        /// </summary>
        Task<Dictionary<string, T>> GetAllHashValues<T>(string key);
    }

    public interface IWriteHashCache
    {
        /// <summary>
        /// Delete keys from hash
        /// </summary>
        Task DeleteHashValues(string key, IEnumerable<string> hashKeys, bool wait);

        /// <summary>
        /// Set hash value
        /// </summary>
        Task<bool> SetHashValue(string key, string hashKey, string value, bool wait);

        /// <summary>
        /// Set hash value. Generic version
        /// </summary>
        Task<bool> SetHashValue<T>(string key, string hashKey, T value, bool wait);
    }

    public interface IScanKeys
    {
        Task<List<string>> KeyScan(string match, int count);
    }

    public interface ICacheDataService :
        IScanKeys,
        IReadCache,
        IWriteCache,
        IReadSetCache,
        IWriteSetCache,
        IReadHashCache,
        IWriteHashCache
    {
    }
}