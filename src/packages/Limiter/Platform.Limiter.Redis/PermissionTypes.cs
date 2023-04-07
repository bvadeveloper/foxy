using System;

namespace Platform.Limiter.Redis;

/// <summary>
/// User types for rate limiting
/// </summary>
public enum PermissionTypes
{
    Admin,
    Advanced,
    Newcomer,
    Default,
}