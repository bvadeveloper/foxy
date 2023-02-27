using MemoryPack;
using Platform.Contract.Messages.Routes;

namespace Platform.Contract.Messages.Messages
{
    [MemoryPackable]
    public partial record DomainMessage(string Name) : ITarget, ITelegramRoute
    {
        public string Name { get; set; } = Name;
    }
}