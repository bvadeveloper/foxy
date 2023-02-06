namespace Platform.Contract.Abstractions;

public record Target : ITarget
{
    /// <summary>
    /// Target value: domain name or ip address
    /// </summary>
    public string Value { get; set; }
}