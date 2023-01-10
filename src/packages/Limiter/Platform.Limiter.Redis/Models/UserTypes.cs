namespace Platform.Limiter.Redis.Models;

/// <summary>
/// User types for rate limiting
/// </summary>
public enum UserTypes
{
    Admin,
    Advanced,
    Newcomer,
    Default
}