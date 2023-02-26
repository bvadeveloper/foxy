using Platform.Contract.Abstractions;

namespace Platform.Contract.Models.Bot
{
    public record DomainTarget(string Name) : ITarget, IBotExchange
    {
        public string Name { get; set; } = Name;
    }
}