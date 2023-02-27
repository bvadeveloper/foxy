namespace Platform.Limiter.Redis.Models;

public record PermissionModel
{
    /// <summary>
    /// Using a hash to not keep the original data
    /// </summary>
    public string Hash { get; init; }

    /// <summary>
    /// Bot nickname
    /// </summary>
    public string Nickname { get; init; }

    /// <summary>
    /// Some description for user
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// Type of user
    /// </summary>
    public LimiterTypes Type { get; init; }

    /// <summary>
    /// Request rate for user type
    /// </summary>
    public int RequestRate { get; init; }
}