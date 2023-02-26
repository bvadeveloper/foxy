using Platform.Contract.Abstractions;

namespace Platform.Contract.Models.Bot
{
    public record IpTarget(string Name) : ITarget, IBotExchange
    {
        public string Name { get; set; } = Name;
    }
}