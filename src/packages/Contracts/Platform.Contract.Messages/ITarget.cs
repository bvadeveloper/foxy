using MemoryPack;
using Platform.Contract.Messages.Messages;

namespace Platform.Contract.Messages
{
    [MemoryPackable]
    [MemoryPackUnion(0, typeof(DomainMessage))]
    [MemoryPackUnion(1, typeof(IpMessage))]
    public partial interface ITarget
    {
        string Name { get; set; }
    }
}