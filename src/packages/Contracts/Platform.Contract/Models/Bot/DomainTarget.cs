using Platform.Contract.Abstractions;

namespace Platform.Contract.Models.Bot
{
    public class DomainTarget : ITarget, IBotExchange
    {
        public DomainTarget(string name) => Name = name;
        public string Name { get; set; }
    }
}