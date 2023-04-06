using System;

namespace Platform.Limiter.Redis;

/// <summary>
/// User types for rate limiting
/// </summary>
[Flags]
public enum PermissionTypes : short
{
    Admin,
    Advanced,
    Newcomer,
    Default,
    
    NotDefault = Admin | Advanced | Newcomer
}