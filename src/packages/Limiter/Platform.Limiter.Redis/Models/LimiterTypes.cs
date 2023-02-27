namespace Platform.Limiter.Redis.Models;

/// <summary>
/// User types for rate limiting
/// </summary>
public enum LimiterTypes
{
    Admin,
    Advanced,
    Newcomer,
    Default
}