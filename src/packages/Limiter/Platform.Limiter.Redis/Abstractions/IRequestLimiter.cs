using System.Threading.Tasks;

namespace Platform.Limiter.Redis.Abstractions;

public interface IRequestLimiter
{
    Task<bool> Acquire(string value);
}