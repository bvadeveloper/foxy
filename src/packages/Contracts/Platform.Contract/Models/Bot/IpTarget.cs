using Platform.Contract.Abstractions;

namespace Platform.Contract.Models.Bot
{
    public class IpTarget : ITarget, IBotExchange
    {
        public IpTarget(string name) => Name = name;
        public string Name { get; set; }
    }
}